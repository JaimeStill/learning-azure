REGION_NAME=eastus

# | tr -d '\r' required for WSL bash
az aks get-versions \
    --location $REGION_NAME \
    --query 'orchestrators[?!isPreview] | [-1].orchestratorVersion' \
    --output tsv | tr -d '\r'