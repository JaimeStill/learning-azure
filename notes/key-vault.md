# Azure Key Vault

Azure Key Vault is a *secret store*: a centralized cloud service for storing app secrets; configuration values like passwords and connection strings that must remain secure at all times. Key Vault helps you control your apps' secrets by keeping them in a single central location and providing secure access, permissions control, and access logging.

The main benefits of using Key Vault are:

* Separation of sensitive app information from other configuration and code, reducing the risk of accidental leaks
* Restricted secret access policies tailored to the apps and individuals that need them
* Centralized secret storage, meaning required changes only have to be made in one place
* Access logging and monitoring to help you understand how and when secrets are accessed

Secrets are stored in individual *vaults*, which are Azure resources used to group secrets together. Secret access and vault management is accomplished via a REST API, which is also supported by all of the Azure management tools, and client libraries available for many popular languages. Every vault has a unique URL where its API is hosted.

## Secrets

In Key Vault, a secret is a name-value pair of strings. Secret names must be 1-127 characters long, contain only alphanumeric characters and dashes, and must be unique within a vault. A secret value can be any UTF-8 string up to 25 KB in size.

> Secret names don't need to be considered especially secret themselves. You can store them in your app's configuration if your implementation calls for it. The same is true of vault names and URLs.

> Key Vault supports two additional kinds of secrets beyond strings - *keys* and *certificat`es* - and provides useful functionality specific to their use cases.

## Vault Authentication and Permissions

Key Vault's API uses Azure Active Directory to authenticate users and apps. Vault access policies are based on *actions* and are applied across an entire vault. For example, an app with **Get** (read secret values), **List** (list names of all secrets), and **Set** (create or update secret values) permissions to a vault can create secrets, list all secret names, and get and set all secret values in that vault.

*All* actions performed on a vault require authentication and authorization; there's no way to grant any kind of anonymous access.

Developers will usually only need **Get** and **List** permissions to a development-environment vault. Some engineers will need full permissions to change and add secrets, when necessary.

For apps, often only **Get** permissions are required. Some apps may require **List** depending on the way the app is implemented.

## Create Key Vaults For Your Applications

A best practice is to create a separate vault for each deployment environment of each of your applications, such as development, test, and production. You can use a single vault to store secrets for multiple apps and environments, but the impact of an attacker gaining read access to a vault increases with the number of secrets in the vault.

> If you use the same names for secrets across different environments for an application, the only environment-specific configuration you need to change in your app is the vault URL.

Creating a vault requires no initial configuration. Your user identity is automatically granted the full set of secret management permissions, and you can start adding secrets immediately. After you have a vault, you can add and manage secrets from any Azure administrative interface, including the Azure portal, the Azure CLI, and Azure PowerShell. When you setup your application to use the vault, you'll need to assign the correct permissions to it.

## Vault Authentication with Managed Identities for Azure Resources

Azure Key Vault uses **Azure Active Directory** (Azure AD) to authenticate users and apps that try to access the vault. To grant our web app access to the vault, we first need to register our app with Azure Active Directory. Registering creates an identity for the app. After the app has an identity, we can assign vault permissions to it.

Appas and users authenticate to Key Vault using an Azure AD authentication token. Getting a token from Azure AD requires a secret or certificate, because anyone with a token could use the app identity to access all of the secrets in the vault.

Our app secrets are secure in the vault, but we still ened to keep a secret or certificate outside of the vault to access them This is called the *bootstrapping problem*, and Azure has a solution for it.

### Managed Identities for Azure Resources

Managed Identities for Azure resources is an Azure feature your app can use to access Key Vault and other Azure services without having to manage a single secret outside of the vault. Using a managed identity is a simple and secure way to take advantage of Key Vault from your web app.

When you enable managed identity on your web app, Azure activates a separate token-granting REST service specifically for your app to use. Your app will request tokens from this service instead of directly from Azure AD. Your app needs to use a secret to share this service, but that secret is injected into your app's environment variables by App Service when it starts up. You don't need to manage or store this secret value anywhere, and nothing outside of your app can access this secret or the managed identity token service endpoint.

Managed identities are available in all editions of Azure AD, including the Free edition included with an Azure subscription. Using it in App Service has no extra cost and requires no configuration, adn you can enable or disable it on an app at any time. Enabling a managed identity for a web app requires only a single Azure CLI command with no configuration.

## Read Secrets in an ASP.NET Core App

Azure Key Vault API is a REST API that handles all management and usage of keys and vaults. Each secret in a vault has a unique URL, and secret values are retrieved with HTTP GET requests.

The official Key Vault client for .NET Core is the `SecretClient` class in the `Azure.Security.KeyVault.Secrets` NuGet package; however, you don't need to use it directly. With ASP.NET Core's `AddAzureKeyVault` method, you can load all the secrets from a vault into the Configuration API at startup. This technique lets you access all of your secrets by name using the same `IConfiguration` interface you use for the rest of your configuration. Apps that use `AddAzureKeyVault` require both **Get** and **List** permissions to the vault.

> Regardless of the framework or language you use to build your app, you should design it to cache secret values locally or load them into memory at startup, unless you have a specific reason not to. Reading them directly from the vault every time you need them is unnecessarily slow and expensive.

`AddAzureKeyVault` only requires the vault name as an input, which we'll get from our local app configuration. It also automatically handles managed identity authentication. When used in an app deplyoed to Azure App Service with managed identities for Azure resources enabled, it will detect the managed identities token service and use it to authenticate. It's a good fit for most scenarios and implements all best practices.