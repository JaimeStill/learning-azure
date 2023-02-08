# Optimize Your Web Applications By Caching Read-Only Data With Redis

https://learn.microsoft.com/en-us/training/modules/optimize-your-web-apps-with-redis/

## Setup

1. Install the Azure CLI and login

    ```bash
    curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

    az login --use-device-code
    ```

2. Install `redis-tools` and `stunnel`:

    ```bash
    sudo apt install redis-tools stunnel4
    ```

## Create Azure Cache

1. Create the resource group:

    ```bash
    az group create \
        --name redis-rg \
        --location eastus
    ```

2. Create the Azure Cache for Redis instance:

    ```bash
    az redis create \
        --resource-group redis-rg \
        --name jps-cache \
        --sku Basic \
        --vm-size c0 \
        --location eastus
    ```

## Configure stunnel to Connect to Azure Cache

1. Create a new stunnel configuration:

    ```bash
    sudo touch /etc/stunnel/azure-cache.conf
    sudo nano /etc/stunnel/azure-cache.conf
    ```

    Add the following:

    ```
    pid = /tmp/stunnel.pid
    delay = yes
    [redis-cli]
        client = yes
        accept = 127.0.0.1:8000
        connect = jps-cache.redis.cache.windows.net:6380
    ```

2. Start stunnel and connect

    ```bash
    sudo service stunnel4 start

    CACHE_KEY=$(az redis list-keys \
            --resource-group redis-rg \
            --name jps-cache \
            --query "primaryKey" | tr -d '"')

    redis-cli \
        -h localhost \
        -p 8000 \
        -a $CACHE_KEY    
    ```

## Connect an App to the Cache

1. Create a net .NET Core Console application:

    ```bash
    dotnet new console \
        --output src/ \
        --name SportsTracker
    ```

2. Create `appsettings.json` in the root of the project:

    > The connection string should be protected in your application. If the application is hosted on Azure, consider using an Azure Key Vault to store the value.
    > 
    > For the purposes of this exercise, it's implied that you'll create `appsettings.json` each time you run the exercise.

    Capture your connection string in the following convention:

    ```
    <cache-name>.redis.cache.windows.net:6380,password=<access-key>,ssl=True,abortConnect=False
    ```

    * `<cache-name>` is the name of the Azure Cache: `jps-cache`.
    * `<access-key>` can be found with the following:
        ```bash
        az redis list-keys \
            --resource-group redis-rg \
            --name jps-cache \
            --query "primaryKey" | tr -d '"'
        ```

    Add the following to `appsettings.json`:

    ```json
    {
        "CacheConnection": "[connection-string]"
    }
    ```

3. Modify [`SportsTracker.csproj`](./src/SportsTracker.csproj) to include `appsettings.json` in the output directory;

    ```bash
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        ...
      </PropertyGroup>

      <ItemGroup>
        <None Update="appsettings.json">
          <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
      </ItemGroup>
    </Project>
    ```

4. Configure [`.gitignore`](./src/.gitignore):

    ```gitignore
    appsettings.json
    bin/
    obj/
    ```

5. Add JSON configuration support:

    ```bash
    dotnet add package Microsoft.Extensions.Configuration.Binder
    dotnet add package Microsoft.Extensions.Configuration.Json
    ```

6. Setup [`Program.cs`](./src/Program.cs) to read configuration:

    ```cs
    using Microsoft.Extensions.Configuration;

    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
    ```

7. Read the connection string from the configuration:

    ```cs
    string? connection = config.GetValue<string>("CacheConnection");
    ```

8. Add support for the Redis cache .NET client:

    ```bash
    dotnet add package StackExchange.Redis
    ```

    Add `using` statement to [`Program.cs`](./src/Program.cs):

    ```cs
    using StackExchange.Redis;
    ```

9. Connect to the cache:

    ```cs
    using ConnectionMultiplexer cache = ConnectionMultiplexer.Connect(connection);
    ```

10. Add a value to the cache:

    ```cs
    IDatabase db = cache.GetDatabase();

    bool setValue = await db.StringSetAsync("test:key", "cache this");
    Console.WriteLine($"SET: {setValue}");
    ```

11. Get a value from the cache:

    ```cs
    string? getValue = await db.StringGetAsync("test:key");

    if (getValue is not null)
        Console.WriteLine($"GET: {getValue}");
    ```

12. Increment a value:

    ```cs
    long newValue = await db.StringIncrementAsync("counter", 50);
    Console.WriteLine($"INCR new value = {newValue}");
    ```

13. Ping the server then flush the database:

    ```cs
    RedisResult result = await db.ExecuteAsync("ping");
    Console.WriteLine($"PING = {result.Type} : {result}");

    result = await db.ExecuteAsync("flushdb");
    Console.WriteLine($"FLUSHDB = {result.Type} : {result}");
    ```

Final [`Program.cs`](./src/Program.cs):

```cs
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

string? connection = config.GetValue<string>("CacheConnection");

if (connection is not null)
{
    using ConnectionMultiplexer cache = ConnectionMultiplexer.Connect(connection);

    IDatabase db = cache.GetDatabase();

    bool setValue = await db.StringSetAsync("test:key", "cache this");
    Console.WriteLine($"SET: {setValue}");

    string? getValue = await db.StringGetAsync("test:key");

    if (getValue is not null)
        Console.WriteLine($"GET: {getValue}");

    long newValue = await db.StringIncrementAsync("counter", 50);
    Console.WriteLine($"INCR new value = {newValue}");

    RedisResult result = await db.ExecuteAsync("ping");
    Console.WriteLine($"PING = {result.Type} : {result}");

    result = await db.ExecuteAsync("flushdb");
    Console.WriteLine($"FLUSHDB = {result.Type} : {result}");
}
```

### Serialize an Object to Cache

1. Create a [`CacheData.cs`](./src/CacheData.cs) record for storing data:

    ```cs
    namespace SportsTracker;

    public record CacheData
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }

        public CacheData(string name, int value, DateTime date)
        {
            Name = name;
            Value = value;
            Date = date;
        }

        public override string ToString() =>
            $"{Name}:{Value}:{Date.ToString("dd-MMM-yyyy")}";
    }
    ```

2. Add a `using` statement for `SportsTracker` and set / get an instance of `CacheData`:

    ```cs
    using SportsTracker;

    // right after INCR example
    CacheData data = new("Test", 333, DateTime.Now);

    bool setData = await db.StringSetAsync("data:test", JsonSerializer.Serialize(data));
    Console.WriteLine($"SET: {setData}");

    string? getResult = await db.StringGetAsync("data:test");

    if (getResult is not null)
    {
        CacheData? getData = JsonSerializer.Deserialize<CacheData>(getResult);

        if (getData is not null)
            Console.WriteLine($"GET: {getData.ToString()}");
    }
    // PING and FLUSHDB examples
    ```

3. Run and verify output:

    ```
    SET: True
    GET: cache this
    INCR new value = 50
    PING = SimpleString : PONG
    FLUSHDB = SimpleString : OK
    SET: True
    GET: Test:333:07-Feb-2023
    ```

## Clean Up

1. Stop stunnel

    ```bash
    sudo service stunnel4 stop
    ```

2. Delete resource group:

    ```bash
    az group delete \
        -n redis-rg \
        -y
    ```