# Connect an App to Azure Cache for Redis Using .NET Core

In this exercise you will learn how to:

* Create a new Redis Cache instance using Azure CLI commands.
* Create a .NET Core console app to add and retrieve values from the cache using the **StackExchange.Redis** package.

## Create Azure Resources

1. Install the Azure CLI and login

    ```bash
    curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

    az login --use-device-code
    ```

2. Create the resource group:

    ```bash
    az group create \
        --name redis-rg \
        --location eastus
    ```

3. Create the Azure Cache for Redis instance:

    ```bash
    az redis create \
        --resource-group redis-rg \
        --name jpsrediscache \
        --sku Basic \
        --vm-size c0 \
        --location eastus
    ```

## Create the Console Application

1. Create a [console app](./src):

    ```bash
    dotnet new console -n Rediscache -o src/
    ```

2. Add the `StackExchange.Redis` package to the project:

    ```bash
    dotnet add ./src/Rediscache.csproj package StackExchange.Redis
    ```

3. Add a [`.gitignore`](./src/.gitignore) with the following:

    ```gitignore
    bin/
    obj/
    ```

4. Setup [Program.cs](./src/Program.cs):

    ```cs
    using StackExchange.Redis;

    if (args.Length > 0)
    {
        try
        {
            string connection = args[0];

            using ConnectionMultiplexer cache = ConnectionMultiplexer.Connect(connection);
            IDatabase db = cache.GetDatabase();

            RedisResult result = await db.ExecuteAsync("ping");
            Console.WriteLine($"Ping = {result.Type} : {result}");

            bool setValue = await db.StringSetAsync("test:key", "100");
            Console.WriteLine($"SET: {setValue}");

            string? getValue = await db.StringGetAsync("test:key");

            if (getValue is not null)
                Console.WriteLine($"GET: {getValue}");
        }
        catch (Exception ex)
        {
            throw new Exception("An error occured interfacing with Azure Cache for Redis.", ex);
        }
    }
    else
    {
        throw new Exception("No connection string was provided");
    }
    ```

5. Capture your connection string in the following convention:

    ```
    <cache-name>.redis.cache.windows.net:6380,password=<access-key>,ssl=True,abortConnect=False
    ```

    * `<cache-name>` is the name of the Azure Cache: `jpsrediscache`.
    * `<access-key>` can be found with the following:
        ```bash
        az redis list-keys \
            --resource-group redis-rg \
            --name jpsrediscache \
            --query "primaryKey" | tr -d '"'
        ```

6. Run the cli app:

    ```bash
    # provide your connection string
    dotnet run <connection-string>
    ```

    Output:

    ```
    Ping = SimpleString : PONG
    SET: True
    GET: 100
    ```

## Clean Up Resources

```bash
az group delete \
    -n redis-rg \
    -y
```