# Work With Mutable and Partial Data in Azure Cache for Redis

https://learn.microsoft.com/en-us/training/modules/work-with-mutable-and-partial-data-in-a-redis-cache/

In this module you will:

* Group multiple operations into a transaction
* Set an expiration time on your data
* Manage out-of-memory conditions
* Use the cache-aside pattern
* Use the **ServiceStack.Redis** package in a .NET Core console application

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
        --name jps-cache \
        --sku Basic \
        --vm-size c0 \
        --location eastus
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