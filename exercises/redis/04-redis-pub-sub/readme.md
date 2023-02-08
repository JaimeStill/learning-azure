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

3. Open two terminal instances and connect to the Redis cache in each:

    ```bash
    redis-cli \
        -h localhost \
        -p 8000 \
        -a $CACHE_KEY
    ```

## Subscribe and Publish Messages to a Known Channel

1. In the **first** terminal, listen for messages on the **org.shipping.alerts** channel:

    ```redis
    SUBSCRIBE org.shipping.alerts
    ```

    Output:

    ```
    Reading messages... (press ENTER to quit)
    1) "subscribe"
    2) "org.shipping.alerts"
    3) (integer) 1
    ```

2. In the **second** terminal, send new messages:

    ```redis
    PUBLISH org.shipping.alerts labelprint-sdf9878

    PUBLISH org.shipping.alerts packagesent-sdf9878
    ```

3. Verify outputs in the **first** terminal:

    ```
    1) "message"
    2) "org.shipping.alerts"
    3) "labelprint-sdf9878"
    1) "message"
    2) "org.shipping.alerts"
    3) "pacakgesent-sdf9878
    ```

4. <kbd>Ctrl + C</kbd> in the **first** terminal. This will kill the `redis-cli` connection and require you to reconnect.

## Subscribe to a Channel Pattern and Listen for Messages

1. In the **first** terminal, listen for message on the **org.inventory.\*** channel:

    ```redis
    PSUBSCRIBE org.inventory.*
    ```

    Output:

    ```
    1) "psubscribe"
    2) "org.inventory.*"
    3) (integer) 1
    ```

2. In the **second** terminal, new messages:

    ```redis
    PUBLISH org.inventory.empty item-sku-318947

    PUBLISH org.shipping.sent order-dsy3821
    ```

3. Observe that the **first** terminal only received the first message:

    ```
    1) "pmessage"
    2) "org.inventory.*"
    3) "org.inventory.empty"
    4) "item-sku-318947"
    ```

4. Close both terminal sessions.

## Broker Messages Using Streams

1. Open a terminal and connect to the Redis cache:

    ```bash
    redis-cli \
        -h localhost \
        -p 8000 \
        -a $CACHE_KEY
    ```

2. Use the `XADD` command to add new entries:

    ```redis
    XADD org.logs.clientapp 1324092248593-0 device-id mobile error unknown-crash

    XADD org.logs.clientapp 1481945061467-0 worker-process 1788 status success
    ```

    Output from both commands:

    ```
    "1324092248593-0"

    "1481945061467-0"
    ```

3. Use the `XADD` command to add another new entry with an automatically generated identifier:

    ```redis
    XADD org.logs.clientapp * application-status started
    ```

    Output (will differ each time):
    
    ```
    "1675878725191-0"
    ```

### Retrieve and Count all Entries in a Stream

1. Count the number of entries in the **org.logs.clientapp** stream:

    ```redis
    XLEN org.logs.clientapp
    ```

    Output:

    ```
    (integer) 3
    ```

3. Get a range of all data in the **org.logs.clientapp** stream:

    ```redis
    XRANGE org.logs.clientapp - +
    ```

    Output:

    ```
    1) 1) "1324092248593-0"
       2) 1) "device-id"
          2) "mobile"
          3) "error"
          4) "unknown-crash"
    2) 1) "1481945061467-0"
       2) 1) "worker-process"
          2) "1788"
          3) "status"
          4) "success"
    3) 1) "1675878725191-0"
       2) 1) "application-status"
          2) "started"
    ```

### Retrieve a Subset of Entries in a Stream

1. Get all values up to and including **1481945061467-0**:

    ```redis
    XRANGE org.logs.clientapp - 1481945061467-0
    ```

    Output:

    ```
    1) 1) "1324092248593-0"
       2) 1) "device-id"
          2) "mobile"
          3) "error"
          4) "unknown-crash"
    2) 1) "1481945061467-0"
       2) 1) "worker-process"
          2) "1788"
          3) "status"
          4) "success"
    ```

2. Get all entries starting from **1481945061467-0**:

    ```redis
    XRANGE org.logs.clientapp 1481945061467-0 +
    ```

    Output:

    ```
    1) 1) "1481945061467-0"
       2) 1) "worker-process"
          2) "1788"
          3) "status"
          4) "success"
    2) 1) "1675878725191-0"
       2) 1) "application-status"
          2) "started"
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