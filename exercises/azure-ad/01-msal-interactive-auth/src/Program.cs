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