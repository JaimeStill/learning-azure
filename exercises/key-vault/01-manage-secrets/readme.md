# Manage Secrets In Your Server Apps With Azure Key Vault

https://learn.microsoft.com/en-us/training/modules/manage-secrets-with-azure-key-vault/

In this module you will:

* Create an Azure Key vault and use it to store secret configuration values
* Enable secure access to the vault from an Azure App Service web app with managed identities for Azure resources
* Implement a web application that retrieves secrets from the vault

## Setup

> Complete only if needed

Install the Azure CLI and login:

```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

az login --use-device-code
```

## Create the Key Vault and Add a Secret

1. Create the resource group:

    ```bash
    az group create \
        --name key-vault-rg \
        --location eastus
    ```

2. Create the key vault:

    **Key Vault names must be globally unique, so you'll need to pick a unique name**. Vault names must be 3-24 caharacters long and contain only alphanumeric characters and dashes.

    ```bash
    az keyvault create \
        --resource-group key-vault-rg \
        --location eastus \
        --name jps-vault-dev
    ```

3. Add a secret:

    ```bash
    az keyvault secret set \
        --name SecretPassword \
        --value reindeer_flotilla \
        --vault-name jps-vault-dev
    ```

## Create and Configure the App

1. Create the ASP.NET Core Web API app:

    ```bash
    dotnet new webapi \
        -n KeyVaultApp \
        -o src/
    ```

2. Add and install Azure dependencies:

    ```bash
    dotnet add package Azure.Identity
    dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
    dotnet restore
    ```

3. Add a [`.gitignore`](./src/.gitignore):

    ```.gitignore
    bin/
    obj/
    ```

4. Delete the following unnecessary files:

    * `Controllers/WeatherForecastController.cs`
    * `WeatherForecast.cs`

## Add Code to Load and Use Secrets

1. In [`appsettings.json`](./src/appsettings.json) and [`appsettings.Development.json`](./src/appsettings.Development.json), add the following key:

    ```json
    {
        "VaultName": "jps-vault-dev"
    }
    ```

2. Create [`Extensions/KeyVaultAppExtensions.cs`](./src/Extensions/KeyVaultAppExtensions.cs) file with the following contents to facilitate loading secrets into app configuration:

    ```cs
    using Azure.Identity;

    namespace KeyVaultApp.Extensions;

    public static class KeyVaultAppExtensions
    {
        public static void InitializeKeyVault(this ConfigurationManager config)
        {
            Uri vaultUri = new($"https://{config["VaultName"]}.vault.azure.net/");

            config.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
        }
    }
    ```

3. In [`Program.cs`](./src/Program.cs), add a `using` statement for `KeyVaultApp.Extensions` and add the call to `InitializeKeyVault()` just below the initialization of the `builder` variable:

```cs
using KeyVaultApp.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.InitializeKeyVault();

// remaining code
```

4. Create [`Controllers/SecretTestController.cs`](./src/Controllers/SecretTestController.cs) and create an endpoint for retrieving the **SecretPassword** vault secret:

    ```cs
    using Microsoft.AspNetCore.Mvc;

    namespace KeyVaultApp.Controllers;

    [Route("api/[controller]")]
    public class SecretTestController : Controller
    {
        readonly IConfiguration config;

        public SecretTestController(IConfiguration config)
        {
            this.config = config;
        }

        IActionResult NoSecret(string name) => StatusCode(
            StatusCodes.Status500InternalServerError,
            $"Error: No secret named {name} was found..."
        );

        IActionResult Secret(string name, string value) => Content(
            $"{name}: {value}\n\n" +
            "This is for testing only! Never output a secret " +
            "to a response or anywhere else in a real app!"
        );

        [HttpGet]
        public IActionResult Get()
        {
            string name = "SecretPassword";
            string? value = config[name];

            return value is null
                ? NoSecret(name)
                : Secret(name, value);
        }
    }
    ```

## Configure, Deploy, and Run in Azure

1. Create an app service plan:

    ```bash
    az appservice plan create \
        --resource-group key-vault-rg \
        --name kv-plan \
        --sku FREE \
        --location eastus
    ```

2. Create a web app:

    ```bash
    az webapp create \
        --resource-group key-vault-rg \
        --plan kv-plan \
        --name jps-kv-app
    ```

3. Add Configuration to the App:

    ```bash
    az webapp config appsettings set \
        --resource-group key-vault-rg \
        --name jps-kv-app \
        --settings 'VaultName=jps-vault-dev'
    ```

4. Enable managed identity:

    ```bash
    az webapp identity assign \
        --resource-group key-vault-rg \
        --name jps-kv-app
    ```

5. Capture the `principalId` of the assigned Managed Identity in a Bash variable:

    ```bash
    PRINCIPAL=$(az webapp identity show \
        --resource-group key-vault-rg \
        --name jps-kv-app \
        --query principalId \
        --output tsv)
    ```

6. Grant access to the vault:

    ```bash
    az keyvault set-policy \
        --name jps-vault-dev \
        --secret-permissions get list \
        --object-id $PRINCIPAL
    ```

7. Deploy the web app:

    ```bash
    dotnet publish -o ../pub
    cd ..
    zip -j site.zip ./pub/*

    az webapp deployment source config-zip \
        --resource-group key-vault-rg \
        --name jps-kv-app \
        --src site.zip

    rm -f ./site.zip
    ```

8. Test out the API:

    https://jps-kv-app.azurewebsites.net/api/secretTest

    Output:

    ```
    SecretPassword: reindeer_flotilla

    This is for testing only! Never output a secret to a response or anywhere else in a real app!
    ```

## Clean Up

Delete the resource group:

```bash
az group delete \
    -n key-vault-rg \
    -y
```