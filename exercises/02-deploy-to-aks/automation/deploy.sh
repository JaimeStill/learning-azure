RESOURCE_GROUP=rg-contoso-video
CLUSTER_NAME=aks-contoso-video
LOCATION=eastus

echo "Creating Resource Group: $RESOURCE_GROUP in $LOCATION"

az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION

echo "Creating Azure Cluster: $CLUSTER_NAME"

./scripts/create-aks.sh

echo "Adding AKS Node Pool"

./scripts/add-aks-nodepool.sh

az aks get-credentials \
    --name $CLUSTER_NAME \
    --resource-group $RESOURCE_GROUP

kubectl get nodes

echo "Applying Kubernetes Deployment"

kubectl apply -f ./kubernetes/deployment.yaml

kubectl get deploy contoso-website

echo "Applying Kubernetes ClusterIP Service"

kubectl apply -f ./kubernetes/service.yaml

kubectl get service contoso-website

echo "Getting DNS Zone"

ZONE=$(./scripts/query-dns-zone.sh)

sed "s/{ZONE}/$ZONE/g" ./kubernetes/ingress-template.yaml > ./kubernetes/ingress.yaml

echo "Applying Kubernetes Ingress Controller"

kubectl apply -f ./kubernetes/ingress.yaml

kubectl get ingress contoso-website

echo "Deployment complete!"