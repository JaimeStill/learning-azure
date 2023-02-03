RESOURCE_GROUP=rg-contoso-video
CLUSTER_NAME=aks-contoso-video

az aks show \
    -g $RESOURCE_GROUP \
    -n $CLUSTER_NAME \
    -o tsv \
    --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName