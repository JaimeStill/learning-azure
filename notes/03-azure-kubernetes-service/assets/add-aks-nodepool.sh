az aks nodepool add \
    --resource-group $RESOURCE_GROUP \
    --cluster-name $CLUSTER_NAME \
    --name uspool \
    --node-count 2 \
    --node-vm-size Standard_B2s \
    --os-type Windows