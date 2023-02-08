# Implement Pub / Sub and Streams in Azure Cache for Redis

https://learn.microsoft.com/en-us/training/modules/azure-redis-publish-subscribe-streams/

In this module you will:

* Subscribe to and publish messages using Azure Cache for Redis
* Append entries to a stream and query the stream using Azure Cache for Redis

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