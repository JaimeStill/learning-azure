# Implement Interactive Authentication by Using MSAL.NET

https://learn.microsoft.com/en-us/training/modules/implement-authentication-by-using-microsoft-authentication-library/

In this module you will:

* Register an app with the Microsoft identity platform.
* Create an app that retrieves a token by using the [MSAL.NET](https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-overview) library.

## Register a New Application

Create the app registration:

```bash
az ad app create \
    --display-name secure-cli \
    --is-fallback-public-client \
    --public-client-redirect-uris http://localhost \
    --sign-in-audience AzureADMyOrg 
```

## Setup a Console Application

1. Create the app:

    ```bash
    dotnet new console -o src/ -n SecureCli
    ```

2. Add dependencies:

    ```bash
    dotnet add package Microsoft.Identity.Client
    ```

## Setup Authentication in [`Program.cs`](./src/Program.cs)

3. Add the `using` statement to the top:

    ```cs
    using Microsoft.Identity.Client;
    ```

3. Capture the Application and Tenant IDs for the registered app:

    **Application ID**  

    ```bash
    az ad app list \
        --query "[0].appId" \
        --output tsv
    ```

    **Tenant ID**

    ```bash
    az account show \
        --query tenantId \
        --output tsv
    ```

4. Add the IDs captured in the previous step as constants:

    ```cs
    // supply the values retrieved from Azure CLI
    const string clientId = "{client-id}";
    const string tenantId = "{tenant-id}";
    ```

5. Build out the authorization context:

    ```cs
    IPublicClientApplication app = PublicClientApplicationBuilder
        .Create(clientId)
        .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
        .WithRedirectUri("http://localhost")
        .Build();
    ```

    Code | Description
    -----|------------
    `.Create` | Creates a `PublicClientApplicationBuilder` from a client ID.
    `.WithAuthority` | Adds a known Authority corresponding to an ADFS server. In the code we're specifying the Public cloud, and using the tenant for the app we registered.

6. Acquire a token:

    When you registered the app, it automatically generated an API permission `user.read` for Microsoft.Graph. We'll use that permission to acquire a token:

    ```cs
    string[] scopes = { "user.read" };

    AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

    Console.WriteLine($"Token:\t{result.AccessToken}");
    ```

Complete [`Program.cs`](./src/Program.cs):

```cs
using Microsoft.Identity.Client;

const string clientId = "d4739442-566f-44fa-91e5-e5b051d44533";
const string tenantId = "64819121-d17e-4216-a81e-fa8528635fb8";

IPublicClientApplication app = PublicClientApplicationBuilder
        .Create(clientId)
        .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
        .WithRedirectUri("http://localhost")
        .Build();

string[] scopes = { "user.read" };

AuthenticationResult result = await app
        .AcquireTokenInteractive(scopes)
        .ExecuteAsync();

Console.WriteLine($"Token:\t{result.AccessToken}");
```

## Run the Application

```bash
dotnet run
```

The app will open the default browser and prompt you to select the account you wish to authenticate with.

If this is the first time you've authenticated to the registered app, you will receive a **Permissions requested** notification asking you to approve teh app to read data associated with your account. Select **Accept**:

![permission-consent](https://learn.microsoft.com/en-us/training/wwl-azure/implement-authentication-by-using-microsoft-authentication-library/media/permission-consent.png)

You should see the results similar to the example below in the console:

```
Token:  eyJ0eXAiOiJKV1QiLCJub25jZSI6IlVhU.....
```

## Clean Up

1. Capture the App ID a Bash variable:

    ```bash
    APP_ID=$(az ad app list \
        --query "[0].appId" \
        --output tsv)
    ```

2. Delete the app registration:

    ```bash
    az ad app delete \
        --id $APP_ID
    ```