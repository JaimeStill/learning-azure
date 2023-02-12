# Protect a Web API with the Microsoft Identity Platform

* https://learn.microsoft.com/en-us/azure/active-directory/develop/web-api-quickstart?pivots=devlang-aspnet-core
* https://learn.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-app-configuration
* https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app

**Helpful Scripts**  

```bash
# set the ID of the app registration as a variable
objectid=$(az ad app show \
    --id $appId \
    --query id \
    --output tsv)

# get the app registration object with az rest
az rest \
    --method GET \
    --headers "Content-Type=application/json" \
    --uri https://graph.microsoft.com/v1.0/applications/$objectid/
```

## Create an Configure an Azure AD App Registration

1. Create the app registration and store the `appId` in a Bash variable:

    ```bash
    appId=$(az ad app create \
        --display-name secure-api \
        --public-client-redirect-uris http://localhost \
        --is-fallback-public-client true \
        --query appId \
        --output tsv)
    ```

2. Create a `uuid` and store it as a Bash variable:

    ```bash
    uuid=$(uuidgen)
    ```

3. Initialize the `api` object associated with the app registration to configure the OAuth scopes:

    > Note that the only value being updated is `api.oauth2PermissionScopes`. The other values represent the defaults.

    ```bash
    api=$(echo '{
        "acceptMappedClaims": null,
        "knownClientApplications": [],
        "oauth2PermissionScopes": [{
            "adminConsentDescription": "Access Secure API",
            "adminConsentDisplayName": "Access SecureApi",
            "id": "'$uuid'",
            "isEnabled": true,
            "type": "User",
            "userConsentDescription": "Access Secure API",
            "userConsentDisplayName": "Access SecureApi",
            "value": "access_as_user"
        }],
        "preAuthorizedApplications": [],
        "requestedAccessTokenVersion": 2
    }' | jq .)
    ```

4. Update the app registration with the new scopes and an identifier URI:

    ```bash
    az ad app update \
        --id $appId \
        --identifier-uris api://$appId \
        --set api="$api"
    ```

## Create the Solution and Projects

1. Create the [`src/SecureApi.sln](./src/SecureApi.sln) solution:

    ```bash
    dotnet new sln -o src/ -n SecureApi
    ```

2. Create the Web API project:

    ```bash
    dotnet new webapi -o SecureApi
    ```

3. Add the **Microsoft.Identity.Web** package the API project:

    ```bash
    dotnet add package Microsoft.Identity.Web
    ```

3. Create the CLI project:

    ```bash
    dotnet new console -o SecureCli
    ```

4. Add the **Microsoft.Identity.Client** package to the CLI project:

    ```bash
    dotnet add package Microsoft.Identity.Client
    ```

5. Add the projects to the solution:

    ```bash
    dotnet sln add ./SecureApi
    dotnet sln add ./SecureCli
    ```

## Configure the API

1. Define the `AzureAd` configuration in [`appsettings.json`](./src/SecureApi/appsettings.json):

    ```js
    {
        "AzureAd": {
            "Instance": "https://login.microsoftonline.com/",
            "Domain": "qualified.domain.name",
            "ClientId": "7d991a88-b796-4d81-a765-09867b13984d",
            "TenantId": "common"
        },
        // remaining configuration
    }
    ```

2. Add authorization to the [`WeatherForecastController.cs`](./src/SecureApi/Controllers/WeatherForecastController.cs) controller:

    Add the `[Authorize]` attribute to the top of the controller definition:

    ```cs
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    ```

    Define the authorization scope just above the constructor:

    ```cs
    static readonly string[] scopeRequiredByApi = { "access_as_user" };
    ```

    Add verification to the top of the `Get` action method:

    ```cs
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
        
        // action logic
    }
    ```

3. Configure the [`Program.cs`](./src/SecureApi/Program.cs) pipeline to support authentication:

    Add the Authentication service configuration:

    ```cs
    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
    ```

    Add Authentication to the middleware pipeline above `.UseAuthorization()`:

    ```cs
    app.UseAuthentication();
    app.UseAuthorization();
    ```

## Configure the CLI app:

All of the following steps pertain to defining [`Program.cs`](./src/SecureCli/Program.cs)

1. Add the `using` statements:

    ```cs
    using System;
    using System.Net.Http;
    using Microsoft.Identity.Client;
    ```

2. Configure the authentication configuration variables:

    ```cs
    const string clientId = "7d991a88-b796-4d81-a765-09867b13984d";
    const string tenantId = "common";
    string[] scopes = { $"api://{clientId}/access_as_user"};
    ```

3. Build the `IPublicClientApplication`:

    ```cs
    IPublicClientApplication app = PublicClientApplicationBuilder
        .Create(clientId)
        .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
        .WithRedirectUri("http://localhost")
        .Build();
    ```

4. Interactively acquire an authentication token:

    ```cs
    AuthenticationResult result = await app
        .AcquireTokenInteractive(scopes)
        .ExecuteAsync();
    ```

5. Initialize an `HttpClient` and configure the authentication header:

    ```cs
    HttpClient client = new();
    client.DefaultRequestHeaders.Authorization = new("Bearer", result.AccessToken);
    ```

6. Call a request to the API endpoint and log the response to the console:

    ```cs
    HttpResponseMessage response = await client.GetAsync("http://localhost:5205/weatherForecast");

    Console.WriteLine(await response.Content.ReadAsStringAsync());
    ```

## Verify API Authentication

1. Open a terminal, run the API app:

    ```bash
    src/SecureApi$ dotnet run
    ```

    Output:

    ```
    Building...
    info: Microsoft.Hosting.Lifetime[14]
        Now listening on: http://localhost:5205
    info: Microsoft.Hosting.Lifetime[0]
        Application started. Press Ctrl+C to shut down.
    info: Microsoft.Hosting.Lifetime[0]
        Hosting environment: Development
    info: Microsoft.Hosting.Lifetime[0]
        Content root path: /learning-azure/exercises/azure-ad/02-msal-protect-web-api/src/SecureApi
    ```

2. Open a second terminal and run the CLI app:

    ```bash
    src/SecureCli$ dotnet run
    ```

    You will be prompted to login with a Microsoft account:

    ![image](https://user-images.githubusercontent.com/14102723/218341395-ac81f90c-65fd-4ed7-ad5f-83277d453e20.png)

    After providing your credentials, you will be prompted to allow the app to access your info:

    ![image](https://user-images.githubusercontent.com/14102723/218341463-2e431822-b55b-416e-838f-c5a340ba4654.png)

    If you click yes, the browser window will provide an **Authentication complete** message:

    ![image](https://user-images.githubusercontent.com/14102723/218341525-c6d2921e-e545-48c2-9ddf-049ea65fcf6e.png)

    and you should see the following output in the terminal:

    ```
    [{"date":"2023-02-13","temperatureC":-6,"temperatureF":22,"summary":"Warm"},{"date":"2023-02-14","temperatureC":9,"temperatureF":48,"summary":"Freezing"},{"date":"2023-02-15","temperatureC":-14,"temperatureF":7,"summary":"Scorching"},{"date":"2023-02-16","temperatureC":-13,"temperatureF":9,"summary":"Bracing"},{"date":"2023-02-17","temperatureC":19,"temperatureF":66,"summary":"Chilly"}]
    ```