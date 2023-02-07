# Azure Container Instances

https://learn.microsoft.com/en-us/training/modules/run-docker-with-azure-container-instances/

Containers offer a standardized and repeatable way to package, deploy, and manage cloud applications. Azure Container Instances let you run a container in Azure without managing virtual machines and without a higher-level service.

Azure Container Instances are useful for scenarios that can operate in isolated containers, including simple applications, task automation, and build jobs. Here are some benefits:

* **Fast startup**: Launch containers in seconds.
* **Per second billing**: Incur costs only while the container is running.
* **Hypervisor-level security**: Isolate your application as completely as it would be in a VM.
* **Custom sizes**: Specify exact values for CPU cores and memory.
* **Persistent storage**: Mount Azure Files shares directly to a container to retrieve and persist state.
* **Linux and Windows**: Schedule both Windows and Linux containers using the same API.

For scenarios where you need full container orchestration, including service discovery across multiple containers, automatic scaling, and coordinated application upgrades, Azure Kubernetes Service (AKS) is recommended.

## Run Azure Container Instances

Here, you'll create a container in Azure and expose it to the Internet with a fully qualified domain name (FQDN).

### Create a Container

1. Install the Azure CLI:

    ```bash
    curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
    ```

2. Login to the Azure CLI:

    ```bash
    az login --use-device-code
    ```

3. Create an Azure Resource Group:

    ```bash
    az group create --name learn-deploy-aci-rg --location eastus
    ```

4. Create a DNS name variable in Bash:

    ```bash
    DNS_NAME_LABEL=aci-demo-$RANDOM
    ```

5. Create and start a container instance:

    ```bash
    az container create \
        --resource-group learn-deploy-aci-rg \
        --name hello-container \
        --image mcr.microsoft.com/azuredocs/aci-helloworld \
        --ports 80 \
        --dns-name-label $DNS_NAME_LABEL \
        --location eastus
    ```

6. Check the status of the container:

    ```bash
    az container show \
        --resource-group learn-deploy-aci-rg \
        --name hello-container \
        --query "{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" \
        --output table
    ```

    FQDN | ProvisioningState
    -----|------------------
    aci-demo-23892.eastus.azurecontainer.io | Succeeded

7. Open the URL http://aci-demo-{number}.eastus.azurecontainer.io in the browser to verify the container is running:

    ![image](https://user-images.githubusercontent.com/14102723/216423623-14cdd328-3a03-40c2-a82d-2cf021c541c3.png)

## Control Restart Behavior

The ease and speed of deploying containers in Azure Container Instances makes it a great fit for executing run-once tasks like image rendering or building and testing applications.

With a configurable restart policy, you can specify that your containers are stopped when their processes have completed. Because container instances are billed by the second, you're charged only for the compute resources used while the container executing your task is running.

Azure Container Instances have three restart-policy options:

Restart policy | Description
---------------|------------
**Always** | Containers in the container group are always restarted. This policy makes sense for long-running tasks like a web server. This is the **default** setting applied when no restart policy is specified at container creation.
**Never** | Containers in the container group are never restarted. The containers run one time only.
**OnFailure** | Containers in the container group are restarted only when the process executed in the container fails (when it terminates with a nonzero exit code). The containers are run at least once. This policy works well for containers that run short-lived tasks.

### Run a Container to Completion

TO see the restart policy in action, create a container instance from the **azuredocs/aci-wordcount** container image and specify the **OnFailure** restart policy. This container runs a Python script that analyzes the text of Shakespeare's Hamlet, writes the 10 most common words to standard output, then exits.

1. Create and start the container:

    ```bash
    az container create \
        --resource-group learn-deploy-aci-rg \
        --name restart-container \
        --image mcr.microsoft.com/azuredocs/aci-wordcount:latest \
        --restart-policy OnFailure \
        --location eastus
    ```

    Azure Container Instances starts the container and then stops it when its process (a script, in this case) exits. When Azure Container Instances stops a container whose restart policy is **Never** or **OnFailure**, the container's status is set to **Terminated**.

2. Check the container's status:

    ```bash
    az container show \
        --resource-group learn-deploy-aci-rg \
        --name restart-container \
        --query "containers[0].instanceView.currentState.state"
    ```

    Output: `"Terminated"`

3. View the container's logs to examine the output:

    ```bash
    az container logs \
        --resource-group learn-deploy-aci-rg \
        --name restart-container
    ```

    Output:

    ```py
    [('the', 990),
     ('and', 702),
     ('of', 628),
     ('to', 610),
     ('I', 544),
     ('you', 495),
     ('a', 453),
     ('my', 441),
     ('in', 399),
     ('HAMLET', 386)]
    ```

## Set Environment Variables

Environment variables allow you to dynamically configure the application or script the container runs. You can use the Azure CLI, PowerShell, or the Azure portal to set variables when you create the container. Secured environment variables prevent sensitive information from displaying in the container's output.

Here, you'll create an Azure Cosmos DB instance and use environment variables to pass the connection information to an Azure container instance. An application in the container uses the variables to write and read data from Azure Cosmos DB. You'll create both an environment variable and a secured environment variable so you can see the difference between them.

### Deploy Azure Cosmos DB

1. Create a database name variable in Bash:

    ```bash
    COSMOS_DB_NAME=aci-cosomos-db-$RANDOM
    ```

2. Create the Azure Cosmos DB instance:

    ```bash
    COSMOS_DB_ENDPOINT=$(az cosmosdb create \
        --resource-group learn-deploy-aci-rg \
        --name $COSMOS_DB_NAME \
        --query documentEndpoint \
        --output tsv)
    ```

    Endpoint: `https://aci-cosmos-db-27178.documents.azure.com:443/`

3. Get the Azure Cosmos DB connection key and store it in a Vash variable:

    ```bash
    COSMOS_DB_MASTERKEY=$(az cosmosdb keys list \
        --resource-group learn-deploy-aci-rg \
        --name $COSMOS_DB_NAME \
        --query primaryMasterKey \
        --output tsv)
    ```

    Key: `ANb7Dh5OdzGFryuFy8D0c8cQAxW1LnImzPeZ2e1TjcnuNPqsLNZm0rZINt9JSaLtmZEmpvqNeUAbACDbjxdIuw==`

### Deploy a Container That Works With Your Database

1. Create and start the container:

    ```bash
    az container create \
        --resource-group learn-deploy-aci-rg \
        --name aci-demo \
        --image mcr.microsoft.com/azuredocs/azure-vote-front:cosmosdb \
        --ip-address Public \
        --location eastus \
        --environment-variables \
            COSMOS_DB_ENDPOINT=$COSMOS_DB_ENDPOINT \
            COSMOS_DB_MASTERKEY=$COSMOS_DB_MASTERKEY
    ```

    Note the `--environment-variables` argument. This argument specifies environment variables that are passed to the container when the container starts. The container image is configured to look for these environment variables. Here, you'll pass the name of the Azure Cosmos DB endpoint and its connection key.

2. Get the container's public IP address:

    ```bash
    az container show \
        --resource-group learn-deploy-aci-rg \
        --name aci-demo \
        --query ipAddress.ip \
        --output tsv
    ```

    Output: 4.157.61.159

3. Navigate to the container's IP address:

    ![image](https://user-images.githubusercontent.com/14102723/216432289-357b9417-4e30-4e65-b0c6-cfa9632a01cc.png)

### Use Secured Environment Variables to Hide Connection Information

In the previous section, you used two environment variables to create your container. By default, these environment variables are accessible through the Azure portal and command-line tools in plain text.

In this section, you'll learn how to prevent sensitive information, such as connection keys, from being displayed in plain text.

1. Display your container's environment variables:

    ```bash
    az container show \
        --resource-group learn-deploy-aci-rg \
        --name aci-demo \
        --query containers[0].environmentVariables
    ```

    Output:

    ```json
    [
      {
        "name": "COSMOS_DB_ENDPOINT",
        "secureValue": null,
        "value": "https://aci-cosmos-db-27178.documents.azure.com:443/"
      },
      {
        "name": "COSMOS_DB_MASTERKEY",
        "secureValue": null,
        "value": "ANb7Dh5OdzGFryuFy8D0c8cQAxW1LnImzPeZ2e1TjcnuNPqsLNZm0rZINt9JSaLtmZEmpvqNeUAbACDbjxdIuw=="
      }
    ]
    ```

    Although these values don't appear to your users through the voting application, it's a good security practice to ensure that sensitive information (such as connection keys) isn't stored in plain text.

    Secure environment variables prevent clear text output. To use secure environment variables, you'll use the `--secure-environment-variables` argument instaed of the `--environment-variables` argument.

2. Delete the previous container:

    ```bash
    az container delete \
        --resource-group learn-deploy-aci-rg \
        --name aci-demo \
        -y
    ```

3. Create a container that uses a secured environment variable for the key:

    ```bash
    az container create \
        --resource-group learn-deploy-aci-rg \
        --name secure-container \
        --image mcr.microsoft.com/azuredocs/azure-vote-front:cosmosdb \
        --ip-address Public \
        --location eastus \
        --environment-variables \
            COSMOS_DB_ENDPOINT=$COSMOS_DB_ENDPOINT \
        --secure-environment-variables \
            COSMOS_DB_MASTERKEY=$COSMOS_DB_MASTERKEY
    ```

3. Show the container's environment variables:

    ```bash
    az container show \
        --resource-group learn-deploy-aci-rg \
        --name secure-container \
        --query containers[0].environmentVariables
    ```

    Output:

    ```json
    [
      {
        "name": "COSMOS_DB_ENDPOINT",
        "secureValue": null,
        "value": "https://aci-cosmos-db-27178.documents.azure.com:443/"
      },
      {
        "name": "COSMOS_DB_MASTERKEY",
        "secureValue": null,
        "value": null
      }
    ]
    ```

    Notice that the `COSMOS_DB_MASTERKEY` variable does not appear in plain text. In fact, it doesn't appear at all. That's OK because these values refer to sensitive information. Here, all you need to know is that environment variables exist.

4. Delete the container:

    ```bash
    az container delete \
        --resource-group learn-deploy-aci-rg \
        --name secure-container \
        -y
    ```

## Use Data Volumes

By default, Azure Container Instances are stateless. If the container crashes or stops, all of its state is lost. To persist state beyond the lifetime of the container, you must mount a volume from an external store.

Here, you'll mount an Azure file share to an Azure container instance so you can store data and access it later.

### Create an Azure File Share

1. Create a storage account name as a Bash variable:

    ```bash
    STORAGE_ACCOUNT_NAME=acistorage$RANDOM
    ```

2. Create a storage account:

    ```bash
    az storage account create \
        --resource-group learn-deploy-aci-rg \
        --name $STORAGE_ACCOUNT_NAME \
        --sku Standard_LRS \
        --location eastus
    ```

3. Place the storage account connection string into an environment variable:

    ```bash
    export AZURE_STORAGE_CONNECTION_STRING=$(az storage account show-connection-string \
        --resource-group learn-deploy-aci-rg \
        --name $STORAGE_ACCOUNT_NAME \
        --output tsv)
    ```

    `AZURE_STORAGE_CONNECTION_STRING` is a special environment variable that's understood by the Azure CLI. The `export` part makes this variable accessible to other CLI commands you'll run shortly.

4. Create a file share named `aci-share` in the storage account:

    ```bash
    az storage share create --name aci-share
    ```

### Get Storage Credentials

To mount an Azure file share as a volume in Azure Container Instances, you need these three values:

* Storage account name
* Share name
* Storage account access key

You already have the first two values. The storage account name is stored in the `STORAGE_ACCOUNT_NAME` Bash variable. You specified `aci-share` as the share name in the previous step. Here, you'll get the remaining value: the storage account access key.

1. Get the storage account key:

    ```bash
    STORAGE_KEY=$(az storage account keys list \
        --resource-group learn-deploy-aci-rg \
        --account-name $STORAGE_ACCOUNT_NAME \
        --query "[0].value" \
        --output tsv)
    ```

    Key value: `ovvVCimjUuc1tQl0G7KN5A81ZUkcbpalflqjCwQs8zxh9vcHuqVjEjftxlJGgp1vL2cQ7LQv4IlQ+AStFeX/fg==`

### Deploy a Container and Mount the File Share

To mount an Azure file share as a volume in a container, you specify the share and volume mount point when you create the container.

1. Create a container that mounts `/aci/logs/` to your file share:

    ```bash
    az container create \
        --resource-group learn-deploy-aci-rg \
        --name aci-files-container \
        --image mcr.microsoft.com/azuredocs/aci-hellofiles \
        --location eastus \
        --ports 80 \
        --ip-address Public \
        --azure-file-volume-account-name $STORAGE_ACCOUNT_NAME \
        --azure-file-volume-account-key $STORAGE_KEY \
        --azure-file-volume-share-name aci-share \
        --azure-file-volume-mount-path /aci/logs/
    ```

2. Get the container's public IP address:

    ```bash
    az container show \
        --resource-group learn-deploy-aci-rg \
        --name aci-files-container \
        --query ipAddress.ip \
        --output tsv
    ```

    Output: `20.241.199.41`

3. Navigate the the container's IP address:

    ![image](https://user-images.githubusercontent.com/14102723/216438985-4411f201-42b8-48c5-9d51-1fd63fc50e7d.png)

4. Enter text in the form and select **Submit**. The action creates a file that contains the text you entered in the Azure file share.

5. Display the files that are contained in your file share:

    ```bash
    az storage file list -s aci-share -o table
    ```

    Name | Content Length | Type | Last Modified
    -----|----------------|------|--------------
    1675368606279.txt | 21 | file | 2023-02-02T20:10:06+00:00

6. Download the file:

    ```bash
    az storage file download -s aci-share -p <filename>
    ```

    [1675368606279.txt](./assets/1675368606279.txt)

7. Print the contents of the file:

    ```bash
    cat 1675368606279.txt
    ```

    Output:
    
    ```
    Store in Azure please
    ```

## Troubleshoot Azure Container Instances

To help you understand basic ways to troubleshoot container instances, here you'll perform some basic operations such as:

* Pulling container logs
* Viewing container events
* Attaching to a container instance

### Get Logs From Your Prior Deployed Container Instance

To see the output from the cats and dogs voting app container:

```bash
az container logs \
    --resource-group learn-deploy-aci-rg \
    --name secure-container
```

Output resemebles:

```
Checking for script in /app/prestart.sh
Running script /app/prestart.sh
Running inside /app/prestart.sh, you could add migrations to this file, e.g.:

#! /usr/bin/env bash

# Let the DB start
sleep 10;
# Run migrations
alembic upgrade head

...
```

### Get Container Events

`az container attach` command provides diagnostic information during container startup. Once teh container has started, it also writes standard output and standard error streams to your local terminal:

```bash
az container attach \
    --resource-group learn-deploy-aci-rg \
    --name secure-container
```

Outut resembles:

```
Container 'aci-demo' is in state 'Running'...
(count: 1) (last timestamp: 2021-09-21 23:48:14+00:00) pulling image "mcr.microsoft.com/azuredocs/azure-vote-front"
(count: 1) (last timestamp: 2021-09-21 23:49:09+00:00) Successfully pulled image "mcr.microsoft.com/azuredocs/azure-vote-front"
(count: 1) (last timestamp: 2021-09-21 23:49:12+00:00) Created container
(count: 1) (last timestamp: 2021-09-21 23:49:13+00:00) Started container

Start streaming logs:
Checking for script in /app/prestart.sh
Running script /app/prestart.sh
…
```

### Execute a Command in Your Container

As you diagnose and troubleshoot issues, you may need to run commands directly on your running container.

1. Start an interactive session on your container:

    ```bash
    az container exec \
        --resource-group learn-deploy-aci-rg \
        --name secure-container \
        --exec-command /bin/sh
    ```

    At this point, you're effectively working inside of the container.

2. Display the contents of the working directory:

    ```bash
    ls
    __pycache__ config_file.cfg main.py prestart.sh static template uwsgi.ini
    ```

3. Explore the system further if you wish. Run `exit` to stop the interactive session.

### Monitor CPU and Memory Usage on Your Container

1. Get the ID of your Azure container instance and store the ID in a Bash variable:

    ```bash
    CONTAINER_ID=$(az container show \
        --resource-group learn-deploy-aci-rg \
        --name secure-container \
        --query id \
        --output tsv)
    ```

2. Retrieve CPU usage information:

    ```bash
    az monitor metrics list \
        --resource $CONTAINER_ID \
        --metrics CPUUsage \
        --output table
    ```

    Note the `--metrics` argument. Here, **CPUUsage** specifies to retrieve CPU usage.

    Output resembles:

    ```
    Timestamp            Name          Average
    -------------------  ------------  -----------
    2021-09-21 23:39:00  CPU Usage
    2021-09-21 23:40:00  CPU Usage
    2021-09-21 23:41:00  CPU Usage
    2021-09-21 23:42:00  CPU Usage
    2021-09-21 23:43:00  CPU Usage      0.375
    2021-09-21 23:44:00  CPU Usage      0.875
    2021-09-21 23:45:00  CPU Usage      1
    2021-09-21 23:46:00  CPU Usage      3.625
    2021-09-21 23:47:00  CPU Usage      1.5
    2021-09-21 23:48:00  CPU Usage      2.75
    2021-09-21 23:49:00  CPU Usage      1.625
    2021-09-21 23:50:00  CPU Usage      0.625
    2021-09-21 23:51:00  CPU Usage      0.5
    2021-09-21 23:52:00  CPU Usage      0.5
    2021-09-21 23:53:00  CPU Usage      0.5
    ```

3. Retrieve memory usage information:

    ```bash
    az monitor metrics list \
        --resource $CONTAINER_ID \
        --metrics MemoryUsage \
        --output table
    ```

    Here, you specified **MemoryUsage** for the `--metrics` argument to retrieve memory usage information.

    Output resembles:

    ```
    Timestamp            Name          Average
    -------------------  ------------  -----------
    2021-09-21 23:43:00  Memory Usage
    2021-09-21 23:44:00  Memory Usage  0.0
    2021-09-21 23:45:00  Memory Usage  15917056.0
    2021-09-21 23:46:00  Memory Usage  16744448.0
    2021-09-21 23:47:00  Memory Usage  16842752.0
    2021-09-21 23:48:00  Memory Usage  17190912.0
    2021-09-21 23:49:00  Memory Usage  17506304.0
    2021-09-21 23:50:00  Memory Usage  17702912.0
    2021-09-21 23:51:00  Memory Usage  17965056.0
    2021-09-21 23:52:00  Memory Usage  18509824.0
    2021-09-21 23:53:00  Memory Usage  18649088.0
    2021-09-21 23:54:00  Memory Usage  18845696.0
    2021-09-21 23:55:00  Memory Usage  19181568.0
    ```

## Clean Up Resources

1. List the Resource Groups in the Azure CLI:

    ```bash
    az group list -o table
    ```

2. Delete the **learn-deploy-aci-rg** resource group:

    ```bash
    az group delete -n learn-deploy-aci-rg -y
    ```