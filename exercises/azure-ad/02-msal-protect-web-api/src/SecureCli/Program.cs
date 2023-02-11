using System;
using System.Net.Http;
using Microsoft.Identity.Client;

const string clientId = "c81019f5-1e0a-4ddb-998d-547af35f0f04";
const string tenantId = "common";
string[] scopes = { $"api://{clientId}/access_as_user"};

IPublicClientApplication app = PublicClientApplicationBuilder
    .Create(clientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
    .WithRedirectUri("http://localhost")
    .Build();

AuthenticationResult result = await app
    .AcquireTokenInteractive(scopes)
    .ExecuteAsync();

HttpClient client = new();
client.DefaultRequestHeaders.Authorization = new("Bearer", result.AccessToken);

HttpResponseMessage response = await client.GetAsync("http://localhost:5205/weatherForecast");

Console.WriteLine(await response.Content.ReadAsStringAsync());