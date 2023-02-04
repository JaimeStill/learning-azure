# Deploy and Run a Containerized Web App with Azure App Service

You can build and deploy Azure-based web apps by using Docker containers. This approach enables you to roll out a web app quickly. Support for continuous delivery ensures that users see the latest build of the app while minimizing administrative overhead.

This exercise shows you how to create and store Docker images in Azure Container Registry. You'll see how to use these images to deploy a web app. Then, you'll learn how to configure continuous deployment so that the web app is redeployed whenever a new version of the image is released.

## Preparation

Download the sample web app and prepare it for use:

```bash
git clone https://github.com/MicrosoftDocs/mslearn-deploy-run-container-app-service.git

mv mslearn-deploy-run-container-app-service/ src/

rm -rf ./src/.git/
rm -rf ./src/node/
```

## Build and Store an Image by Using Azure Container Registry

1. Make sure the [Azure CLI](../../notes/azure-cli.md) is installed and you are logged in.

2. Create a new resource group:

    ```bash
    az group create \
        --name container-app-rg \
        --location eastus
    ```

3. Create the container registry

    ```bash
    az acr create \
        --resource-group container-app-rg \
        --name jpsappregistry \
        --sku Standard \
        --admin-enabled true
    ```

    Set variables for the admin username and password:

    ```bash
    ACR_ADMIN=$(az acr credential show \
        --name jpsappregistry \
        --query "username" \
        | tr -d '"')

    ACR_ADMIN_PW=$(az acr credential show \
        --name jpsappregistry \
        --query "passwords[0].value" \
        | tr -d '"')
    ```

### Build a Docker Image and Upload it to Azure Container Registry

1. Send the folder's contents to the Container Registry to build and store the image using the instructions in its Dockerfile:

    ```bash
    az acr build \
        --registry jpsappregistry \
        --image webimage \
        ./src/dotnet/
    ```

    To get information about the image repository:

    ```bash
    az acr repository show \
        --name jpsappregistry \
        --repository webimage
    ```

    To get the image tag information:

    ```bash
    az acr repository show-tags \
        --name jpsappregistry \
        --repository webimage
    ```

## Create and Deploy a Web App from a Docker Image

1. Create an App Service plan:

    ```bash
    az appservice plan create \
        --name container-app-plan \
        --resource-group container-app-rg \
        --sku F1 \
        --is-linux
    ```

2. Create the web app:

    ```bash
    az webapp create \
        --name jps-container-app \
        --plan container-app-plan \
        --resource-group container-app-rg \
        --docker-registry-server-user $ACR_ADMIN \
        --docker-registry-server-password $ACR_ADMIN_PW \
        --deployment-container-image-name jpsappregistry.azurecr.io/webimage:latest
    ```

4. Navigate to https://jps-container-app.azurewebsites.net/

    ![image](https://user-images.githubusercontent.com/14102723/216739774-46a9e1a6-0f32-42bb-811e-c0304b3ef22c.png)

### Configure Continuous Deployment

1. Configure continuous deployment:

    ```bash
    az webapp deployment container config \
        --resource-group container-app-rg \
        --name jps-container-app \
        --enable-cd true
    ```

2. Capture the Webhook URL in a Bash variable:

    ```bash
    ACR_WEBHOOK=$(az webapp deployment container show-cd-url \
        --resource-group container-app-rg \
        --name jps-container-app \
        --query "CI_CD_URL" \
        | tr -d '"')
    ```
3. Configure the ACR CD webhook:

    ```bash
    az acr webhook create \
        --resource-group container-app-rg \
        --registry jpsappregistry \
        --name deployapp \
        --uri $ACR_WEBHOOK \
        --actions push
    ```

## Modify the Image and Redeploy

1. Replace the default page in the web app:

    ```bash
    cd src/dotnet/SampleWeb/Pages/

    mv Index.cshtml Index.cshtml.old
    mv Index.cshtml.new Index.cshtml

    cd ../../../../
    ```

2. Rebuild the image and push to Container Registry:

    ```bash
    az acr build \
        --registry jpsappregistry \
        --image webimage ./src/dotnet/
    ```

3. Verify that the webhook intercepted the new image:

    ```bash
    az acr webhook list-events \
        --resource-group container-app-rg \
        --registry jpsappregistry \
        --name deployapp \
        --output table
    ```

4. Navigate to https://jps-container-app.azurewebsites.net to see the update:

    ![image](https://user-images.githubusercontent.com/14102723/216740199-5c0b87be-c46a-441d-bdf0-9f23049c692f.png)

## Clean Up Resource

1. Delete the resource group:

    ```bash
    az group delete -n container-app-rg -y
    ```