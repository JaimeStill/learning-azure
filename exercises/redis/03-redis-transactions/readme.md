# Work With Mutable and Partial Data in Azure Cache for Redis

https://learn.microsoft.com/en-us/training/modules/work-with-mutable-and-partial-data-in-a-redis-cache/

In this module you will:

* Group multiple operations into a transaction
* Set an expiration time on your data
* Manage out-of-memory conditions
* Use the cache-aside pattern
* Use the **ServiceStack.Redis** package in a .NET Core console application

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

## Configure `stunnel` to Connect to Azure Cache

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

2. Start stunnel and capture access key:

    ```bash
    sudo service stunnel4 start

    CACHE_KEY=$(az redis list-keys \
        --resource-group redis-rg \
        --name jps-cache \
        --query primaryKey \
        --output tsv)
    ```

## Initialize CLI App

1. Create a new .NET Console app:

    ```bash
    dotnet new console \
        --output src/ \
        --name RedisData
    ```

2. Add the **ServiceStack.Redis** package:

    ```bash
    dotnet add package ServiceStack.Redis
    ```

3. Add and configure [`.gitignore`](./src/.gitignore):

    ```gitignore
    bin/
    obj/
    ```

3. Build out initial [`Program.cs`](./src/Program.cs) shell:

    ```cs
    using ServiceStack.Redis;

    if (args.Length > 0)
    {
        try
        {
            string connection = args[0];

            // Interface with Redis
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred interfacing with Azure Cache for Redis.", ex);
        }
    }
    else
    {
        throw new Exception("No connection string was provided");
    }
    ```

    This setup enables the connection string to be passed in as an argument with the execution of the CLI app.

## Create a Transaction in Azure Cache for Redis

1. Initialize the transaction using the provided connection string:

    ```cs
    bool result = false;

    using RedisClient client = new(connection);
    using IRedisTransaction transaction = client.CreateTransaction();
    ```

2. Schedule the creation of two values:

    ```cs
    transaction.QueueCommand(c => c.Set("favorites:food", "pizza"));
    transaction.QueueCommand(c => c.Set("favorites:drink", "coffee"));
    ```

3. Commit and get the result of the transaction:

    ```cs
    result = transaction.Commit();

    Console.WriteLine(
        result
            ? "Transaction committed"
            : "Transaction failed to commit"
    );
    ```

4. Run the CLI app using the provided [`get-redis-connection.sh`](./scripts/get-redis-connection.sh) script:

    ```bash
    dotnet run $(../scripts/get-redis-connection.sh)
    ```

## Verify Data

1. Connect to the cache server:

    ```bash
    redis-cli \
        -h localhost \
        -p 8000 \
        -a $CACHE_KEY
    ```

2. Check `favorites:food`:

    ```bash
    get favorites:food
    ```

    Output:

    ```
    "\"pizza\""
    ```

3. Check `favorites:drink`:

    ```bash
    get favorites:drink
    ```

    Output:

    ```
    "\"coffee\""
    ```

4. Exit `redis-cli` with the `exit` command.


## Add an Expiration Time

1. Add the following expiration commands before `transaction.Commit()` in [`Program.cs`](./src/Program.cs):

    ```cs
    transaction.QueueCommand(c => ((RedisNativeClient)c).Expire("favorites:food", 15));
    transaction.QueueCommand(c => ((RedisNativeClient)c).Expire("favorites:drink", 15));
    ```

2. Add the following after writing the result to the console to test the expiration:

    ```cs
    Console.WriteLine(
        result
            ? "Transaction committed"
            : "Transaction failed to commit"
    );

    // add here
    string food = client.GetValue("favorites:food");
    string drink = client.GetValue("favorites:drink");

    Console.WriteLine($"Favorite Food: {food} - Favorite Drink: {drink}\n");
    Console.WriteLine("Cached preferences expire in 15 seconds\n");

    for (int i = 15; i > 0; i--)
    {
        Console.WriteLine($"{i} seconds remaining");
        await Task.Delay(1000);
    }

    food = client.GetValue("favorites:food");
    drink = client.GetValue("favorites:drink");
    
    Console.WriteLine("\nfavorites:food and favorites:drink expired");
    Console.WriteLine($"Values: {food} - {drink}");
    ```

3. Run and verify that the values expired:

    ```bash
    dotnet run $(../scripts/get-redis-connection.sh)
    ```

    Output:

    ```
    Transaction committed
    Favorite Food: "pizza" - Favorite Drink: "coffee"

    Cached preferences expire in 15 seconds

    15 seconds remaining
    14 seconds remaining
    13 seconds remaining
    12 seconds remaining
    11 seconds remaining
    10 seconds remaining
    9 seconds remaining
    8 seconds remaining
    7 seconds remaining
    6 seconds remaining
    5 seconds remaining
    4 seconds remaining
    3 seconds remaining
    2 seconds remaining
    1 seconds remaining

    favorites:food and favorites:drink expired
    Values:  - 
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