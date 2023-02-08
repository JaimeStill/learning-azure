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

Key Vault's API uses Azure Active Directory to authenticate users and apps. Vault access policies are based on *actions* and are applied