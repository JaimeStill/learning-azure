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
    dotnet add package Microsoft.Extensions.Configuration.Json
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