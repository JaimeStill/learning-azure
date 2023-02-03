# Azure Container Registry

Azure Container Registry is a managed Docker registry service based on the open-source Docker Registry 2.0. Container Registry is private, hosted in Azure, and allows you to build, store, and manage images for all types of container deployments.

Container images can be pushed and pulled with Container Registry using the Docker CLI or the Azure CLI. Azure portal integration allows you to visually inspect the container images in your container registry. In distributed environments, the Container Registry geo-replication feature can be used to distribute container images to multiple Azure datacenters for localized distribution.

In addition to storing container images, Azure Container Registry Tasks can build container images in Azure. Tasks use a standard Dockerfile to create and store a container image in Azure Container Registry without the need for local Docker tooling. With Azure Contaienr Registry Tasks, you can build on demand or fully automate container image builds using DevOps processes and tooling.S

## Deploy Azure Container Registry

1. Make sure the [Azure CLI](../../notes/azure-cli.md) is installed and you are logged in.

2. Create a new resource group:

    ```bash
    az group create \
        --name learn-acr-rg \
        --location eastus
    ```

    Next, we'll create an Azure container registry by running the `az acr create` command. The container registry name must be unique within Azure and container between 5 and 50 alphanumeric characters.

    In this example, a premium registry SKU is deployed. The premium SKU is required for geo-replication.

3. Define the container registry name as a Bash variable:

    ```bash
    ACR_NAME=jpsregistry
    ```

4. Create the container registry:

    ```bash
    az acr create \
        --resource-group learn-acr-rg \
        --name $ACR_NAME \
        --sku Premium
    ```

## Build Container Images with Azure Container Registry Tasks

Suppose your company makes use of container images to manage compute workloads. You use the local Docker tooling to build your container images.

You can now use Azure Container Registry Tasks to build these container images. Container Registry Tasks also allows for DevOps process integration with automated build on source code commit.

A standard Dockerfile provides build instructions. Azure Container Registry Tasks enables you to reuse any Dockerfile currently in your environment, including multi-staged builds.

1. Create a [Dockerfile](./assets/Dockerfile) with the following contents and save:

    ```bash
    code Dockerfile
    ```

    ```Dockerfile
    FROM    node:9-alpine
    ADD     https://raw.githubusercontent.com/Azure-Samples/acr-build-helloworld-node/master/package.json /
    ADD     https://raw.githubusercontent.com/Azure-Samples/acr-build-helloworld-node/master/server.js /
    RUN     npm install
    EXPOSE  80
    CMD     ["node", "server.js"]
    ```

2. Build the container image from teh Dockerfile:

    ```bash
    az acr build \
        --registry $ACR_NAME \
        --image acrtasks:v1 .
    ```

    > Don't forget the period `.` at the end of the preceding command. It represents the source directory containing the docker file. Beacuse we didn't specify the name of a file with the `--file` parameter, the command looks for a file called **Dockerfile** in the current directory. The command should be executed from the directory the Dockerfile was created in.

3. Verify that the image has been created and stored in the registry:

    ```bash
    az acr repository list \
        --name $ACR_NAME \
        --output table
    ```

    Output:

    ```
    Result
    --------
    acrtasks
    ```

## Deploy Images from Azure Container Registry

We can pull container images from Azure Container Registry using many container management platforms, such as Azure Container Instances, Azure Kubernetes Service, and Docker.

Azure Container Registry doesn't support unauthenticated access and requires authentication for all operations. Registries support two types of identities:

* **Azure Active Directory identities**, including both user and service principals. Access to a registry with an Azure Active Directory identity is role-based, and identities can be assigned one of three roles: **reader** (pull access only), **contributor** (push and pull access), or **owner** (pull, push, and assign roles to other users).
* The **admin account** included with each registry. The admin accuont is disabled by default.

The admin account provides a quick option to try a new registry. You can enable the account and use its username and password in worklfows and apps that need access. After you've confirmed the registry works as expected, you should disable the admin account, and use Azure Active Directory identities exclusively to ensure the security of your registry.

### Enable the Registry Admin Account

1. Enable the admin account on your registry:

    ```bash
    az acr update \
         -n $ACR_NAME \
         --admin-enabled true
    ```

2. Retreive the username and password for the admin account you enabled:

    ```bash
    az acr credential show \
        --name $ACR_NAME
    ```

    Set the admin username as a Bash variable:

    ```bash
    ACR_ADMIN=$(az acr credential show \
        --name $ACR_NAME \
        --query "username" | tr -d '"')
    ```

    Set the admin password as a Bash variable:

    ```bash
    ACR_ADMIN_PW=$(az acr credential show \
        --name $ACR_NAME \
        --query "passwords[0].value" | tr -d '"')
    ```

### Deploy a Container with Azure CLI

1. Deploy a container instance:

    > Using environment variables for `--registry-username` and `--registry-password` does not work currently.  
    > See this [GitHub issue](https://github.com/Azure/azure-cli/issues/7593).

    ```bash
    az container create \
        --resource-group learn-acr-rg \
        --name acr-tasks \
        --image $ACR_NAME.azurecr.io/acrtasks:v1 \
        --registry-login-server $ACR_NAME.azurecr.io \
        --ip-address Public \
        --location eastus \
        --registry-username <username> \
        --registry-password <password>
    ```

2. Get the IP address of the Azure container instance:

    ```bash
    az container show \
        --resource-group learn-acr-rg \
        --name acr-tasks \
        --query ipAddress.ip \
        --output table
    ```

3. Navigate to the IP in the browser:

    ![image](https://user-images.githubusercontent.com/14102723/216631819-10288904-f855-4079-bb18-548d8ab7fd4c.png)

## Replicate a Container Image to Different Azure Regions

Suppose your company has compute workloads deployed to several regions to make sure you have a local presence to serve your distributed customer base.

Your aim is to place a container registry in each region where images are run. This strategy will allow for network-close operations, enabling fast, reliable image layer transfers.

Geo-replication enables an Azure container registry to function as a single registry, serving several regions with multi-master regional registries.

A geo-replicated registry provides the following benefits:

* Single registry/image/tag names can be used across multiple regions
* Network-close registry access from regional deployments
* No extra egress fees, as images are pulled from a local, replicated registry in the same region as your container host
* Single management of a registry across multiple regions

1. Repliate your registry to another region:

    ```bash
    az acr replication create \
        --registry $ACR_NAME \
        --location japaneast
    ```

    Output resembles:

    ```json
    {
      "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myresourcegroup/providers/Microsoft.ContainerRegistry/registries/myACR0007/replications/japaneast",
      "location": "japaneast",
      "name": "japaneast",
      "provisioningState": "Succeeded",
      "regionEndpointEnabled": true,
      "resourceGroup": "myresourcegroup",
      "status": {
        "displayStatus": "Syncing",
        "message": null,
        "timestamp": "2021-11-02T18:47:31.471393+00:00"
      },
      "systemData": {
        "createdAt": "2021-11-02T18:47:31.471393+00:00",
        "createdBy": "username@microsoft.com",
        "createdByType": "User",
        "lastModifiedAt": "2021-11-02T18:47:31.471393+00:00",
        "lastModifiedBy": "useremailid@microsoft.com",
        "lastModifiedByType": "User"
      },
      "tags": {},
      "type": "Microsoft.ContainerRegistry/registries/replications"
      "zoneRedundancy": "Disabled"
    }
    ```

2. Retrieve all container image replicas created:

    ```bash
    az acr replication list \
        --registry $ACR_NAME \
        --output table
    ```

    Output:

    NAME | LOCATION | PROVISIONING STATE | STATUS | REGION ENDPOINT ENABLED
    -----|----------|--------------------|--------|------------------------
    japaneast | japaneast | Succeeded | Ready | True
    eastus | eastus | Succeeded | Ready | True 

## Clean Up Resources

1. Delete the resource group:

    ```bash
    az group delete -n learn-acr-rg -y
    ```