export RESOURCE_GROUP=rg-contoso-video
export CLUSTER_NAME=aks-contoso-video

az aks create \
    --resource-group $RESOURCE_GROUP \
    --name $CLUSTER_NAME \
    --node-count 2 \
    --enable-addons http_application_routing \
    --generate-ssh-keys \
    --node-vm-size Standard_B2s \
    --network-plugin azure \
    --windows-admin-username localadmin # pwd: P@$$Word1234!@#$