REGION_NAME=eastus
RESOURCE_GROUP=rg-akscostsaving
AKS_CLUSTER_NAME=akscostsaving-25841

VERSION=$(source ${BASH_SOURCE%/*}/get-kubernetes-version.sh)

# VERSION=$(az aks get-versions \
#     --location $REGION_NAME \
#     --query 'orchestrators[?!isPreview] | [-1].orchestratorVersion' \
#     --output tsv | tr -d '\r')

echo "Kubernetes version: $VERSION"

az aks create \
    --resource-group $RESOURCE_GROUP \
    --name $AKS_CLUSTER_NAME \
    --location $REGION_NAME \
    --kubernetes-version $VERSION \
    --node-count 2 \
    --load-balancer-sku standard \
    --vm-set-type VirtualMachineScaleSets \
    --generate-ssh-keys