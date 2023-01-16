# Azure CLI

> [Install on Linux](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-linux?view=azure-cli-latest&pivots=apt)

**Download script file to view**

```bash
# install in one line
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# download install script to inspect (if curious)
curl -sL https://aka.ms/InstallAzureCLIDeb --output install-azure-cli.bash
```

**Commands**  

```bash
# login to azure subscription
az login --use-device-code

# install Azure Kubernetes Service CLI
sudo az aks install-cli

# only necessary if you don't have a resource group
az group create --name MyMicroserviceResources --location eastus

# list current resource groups
az group list

# list available locations in a table
az account list-locations -o table

# view azure subscriptions
az account list --all

# set a session subscription
az account set -s NAME_OR_ID

# create AKS cluster in the resource group
az aks create --resource-group RESOURCE_GROUP --name MyMicroserviceCluster --node-count 1 --enable-addons http_application_routing --generate-ssh-keys

# Download credentials to deploy to AKS cluster
az aks get-credentials --resource-group RESOURCE_GROUP --name MyMicroserviceCluster

# delete all resources
az group delete -n MyMicroserviceResources
```