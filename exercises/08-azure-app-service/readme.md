# Host a Web Appliaction with Azure App Service

https://learn.microsoft.com/en-us/training/modules/host-a-web-app-with-azure-app-service/

## Install .NET SDK

> Only required if .NET SDK 6 is not installed

1. Install the LTS .NET SDK:

    ```bash
    sudo apt install dotnet-sdk-6.0
    ```

2. Verify installation:

    ```bash
    dotnet --version
    ```

    Output resembles: `6.0.113`

## Install GitHub CLI

> Only required if not already configured

1. Install the [GitHub CLI](https://snapcraft.io/gh)

    ```bash
    sudo snap install gh
    ```

2. Authenticate to GitHub and Generate an SSL key:

    ```bash
    gh auth login
    # Prompt Selections:
    # Select GitHub.com
    # Select SSH
    # Upload your SSH public key to GitHub
    # Login with a web browser, and follow the instructions
    ```

3. Verify authentication:

    ```bash
    gh auth status
    ```

4. Add generated SSH key for `git` operations:

    ```bash
    eval "$(ssh-agent -s)"
    ssh-add ~/snap/gh/502/.ssh/id_ed25519
    ```

## Generate the Web App and GitHub Repo

1. Generate the [web app](./src):

    ```bash
    dotnet new mvc \
        -n BestBikeApp
        -o src/
    ```

2. Generate a `.gitignore` in the project source:

    ```bash
    code .gitignore
    ```

    ```gitignore
    bin/
    obj/
    ```

3. Initialize git tracking and create a first commit:

    ```bash
    git init
    git add .
    git commit -m "initial"
    ```

4. Create a GitHub repository connected to the web app:

    ```bash
    gh repo create azure-app-svc-deployment \
        --public
    ```

5. Add remote and push to GitHub:

    ```bash
    git remote add origin git@github.com:JaimeStill/azure-app-svc-deployment.git
    git push -u origin main
    ```

## Create the App Service Web App

1. Ensure the [Azure CLI](../../notes/azure-cli.md) is installed and you are logged in.

2. Create a Resource Group:

    ```bash
    az group create \
        --name app-svc-rg \
        --location eastus
    ```

3. Create an App Service plan:

    ```bash
    az appservice plan create \
        --name app-svc-plan \
        --resource-group app-svc-rg \
        --sku FREE \
        --is-linux
    ```

4. Create a web app:

    ```bash
    az webapp create \
        --name app-svc-app \
        --plan app-svc-plan \
        --resource-group app-svc-rg \
        --runtime DOTNETCORE:6.0 \
        --deployment-source-branch main \
        --deployment-source-url <repo-url>
    ```

### Preview Your Web App

1. Get the default host name:

    ```bash
    az webapp list \
        --resource-group app-svc-rg \
        --query "[0].defaultHostName"
    ```

    Output: `app-svc-app.azurewebsites.net`

2. Navigate to the URL in a browser:

    ![image](https://user-images.githubusercontent.com/14102723/216684753-ed8d1d26-55e5-4eb8-9bc3-e005b0efef6b.png)

## Clean Up Resources

1. Delete the resource group:

    ```bash
    az group delete -n app-svc-rg -y
    ```

2. Delete the GitHub repo:

    Before the repository can be deleted, the remote API interface needs to be updated with the `delete_repo` scope:

    ```bash
    gh auth refresh -h github.com -s delete_repo
    # complete authorization
    ```

    Delete the repository:

    ```bash
    gh repo delete azure-app-svc-deployment --confirm
    ```