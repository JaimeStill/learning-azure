RESOURCE_GROUP=rg-akscostsaving
AKS_CLUSTER_NAME=akscostsaving-25841

az aks nodepool scale \
    --resource-group $RESOURCE_GROUP \
    --cluster-name $AKS_CLUSTER_NAME \
    --name batchprocpl \
    --node-count 0