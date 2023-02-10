# Azure Active Directory

Azure Active Directory (Azure AD) is Microsoft' cloud-based identity and access management service. It simplifies authentication for developers by providing identity as a service. It supports industry-standard protocols such as OAuth 2.0 and OpenID Connect.

Azure AD allows users to sign in and view resoures, and it has features to help secure your identities, such as Identity Protection and multi-factor authentication.

Microsoft services such as Azure and Microsoft 365 use Azure AD to store and manage users. Whenever Microsoft 365 needs to verify a user, for example, Azure AD performs all identity and access management.

## Authentication in Azure Active Directory

Users authenticate in two stages:

1. The identity provider verifies the identity of users who exist in the directory. Upon successful authentication, tokens are issued that contain information related to the successful authentication.

2. The user passes those tokens to the application. The application must validate the user's security tokens to ensure that authentication would be successful.

Consider the following scenario:

![azure-ad-open-id-connect](https://learn.microsoft.com/en-us/training/modules/secure-app-with-oidc-and-azure-ad/media/2-azure-ad-open-id-connect.svg)

1. The user requests a secured resource, in this case a web application.

2. The web application redirects the request to the identity provider that requests and checks the user's authentication credentials.

3. If the user sends correct credentials, the provider returns a security token to the user and redirects the user to the resoruce they originally requested.

4. The user sends the security token to the web application.

5. The web application uses the token to verify that the identity provider has performed the authentication.

after this process of authentication, in which the user is positively identified, the web application must authorize the user's access to resources. Authorization is the process by which the web application checks whether the user is permitted to access the requested resource.

This communication flow is built upon the industry-standard protocols of OAuth 2.0 and OpenID Connect.

## OAuth 2.0

OAuth 2.0 is the industry-standard protocol for authorization. It provides specific authorization flows for web, desktop, and mobile applications. The specification was primarily designed to enable users to authorize an application to access data in another application.

Imagine you have an application that stores contact information. You want to allow users with LinkedIn accounts to import their LinkedIn contact information into your application. With OAuth, you can enable this server-to-server communication. Users can authorize your application to access contact information, wihtout needing to share passwords between applications.

OAuth works well for authorization of server-to-server communications, but it doesn't include standards or specifications for authentication. As applications continued to grow their sharing of data and account information between them, the need for a standard framework for single-sign on became evident. This led to the development of OpenID Connect.

## OpenID Connect

OpenID Connect is an authentication layer that's built on top of OAuth 2.0. It includes identity verification methods that are missing from OAuth 2.0. OpenID Connect gives you an access token plus an ID token, which you can send to an application to prove your identity.

The ID token is a JSON Web Token (JWT) and contains information about the authenticated user. THe identity provider signs the token so that applications can verify the authentication by using the provider's public key.

JSON Web Token is an open international standard that defines how applications can exchange data securely as digitally signed messages. The content of each token is not encrypted, but the message is signed with the private key of the provider. By checking the signature with the corresponding public key, applications can prove that the token is issued by the identity provider and has not been tampered with.

![openid-connect-auth-flow](https://learn.microsoft.com/en-us/training/modules/secure-app-with-oidc-and-azure-ad/media/2-openid-connect-auth-flow.svg)

The diagram shows how the client application, the application server, and the identity provider communicate in an OpenID Connect authentication request. The client might be a mobile app or a desktop application. In this case, it's a web browser. THe application server is usually a web server that hosts webpages or a web API. THe identity provider in the middle is Azure AD.

When the web browser goes to the web application, the web server needs the user to be authenticated. It redirects the browser to Azure AD and provides its own client ID, which has been registered in Azure AD. When the user has successfully authenticated against Azure AD, the provider redirects the browser to the URI on the web server.

When you implement OpenID Connect, you must obtain a client ID for your application by creating an application registration in Azure AD. You then copy the client ID into the application's configuration files. In the application registration, you also include the URI of the web application so that Azure AD can redirect the client successfully.

## Microsoft Authentication Library

The Microsoft Authentication Library (MSAL) can be used to provide secure access to Microsoft Graph, other Microsoft APIs, third-party web APIs, or your own web API. MSAL supports many different application architectures and platforms including .NET, JavaScript, Java, Python, Android, and iOS.

MSAL gives you many ways to get tokens, with a consistent API for a number of platforms. Using MSAL provides the following benefits:

* No need to directly use the OAuth libraries or code against the protocol in your application.
* Acquires tokens on behalf of a user or on behalf of an application (when applicable to the platform).
* Maintains a token cache and refreshes tokens for you when they are close to expire. YOu don't need to handle token expiration on your own.
* Helps you specify which audience you want your application to sign in.
* Helps you set up your application from configuration files.
* Helps you troubleshoot your app by exposing actionable exceptions, logging, and telemetry.

### Application Types and Scenarios

Using MSAL, a token can be acquired from a number of application types: web applications, web APIs, single-page apps (JavaScript), mobile and native applications, and daemons and server-side applications. MSAL currently supports thtese platforms and frameworks:

Library | Supported Platforms and Frameworks
--------|-----------------------------------
[MSAL for Android](https://github.com/AzureAD/microsoft-authentication-library-for-android) | Android
[MSAL for Angular](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-angular) | Single-page apps with Angular
[MSAL for iOS and macOS](https://github.com/AzureAD/microsoft-authentication-library-for-objc) | iOS and macOS
[MSAL Go](https://github.com/AzureAD/microsoft-authentication-library-for-go) | Windows, macOS, Linux
[MSAL Java](https://github.com/AzureAD/microsoft-authentication-library-for-java) | Windows, macOS, Linux
[MSAL.js](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-browser) | JavaScript / TypeScript frameworks
[MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) | .NET (Framework, Core, Xamarin, etc)
[MSAL Node](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-node) | Node.js platform
[MSAL Python](https://github.com/AzureAD/microsoft-authentication-library-for-python) | Windows, macOS, Linux
[MSAL React](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-react) | Single-page apps with React

### Authentication Flows

Below are some of the different authentication flows provided by Microsoft Authentication Library (MSAL). These flows can be used in a variety of different application scenarios.

Flow | Description
-----|------------
Authorization code | Native and web apps securely obtain tokens in the name of the user
Client credentials | Service applications run without user interaction
On-behalf-of | The applications calls a service/web API, which in turn calls Microsoft Graph
Implicit | Used in browser-based applications
Device code | Enables sign-in to a device by using another device that has a browser
Integrated windows | Windows computers silently acquire an access token when they are domain joined
Interactive | Mobile and desktop applications call Microsoft Graph in the name of a user
Username/password | The application signs in a user by using their username and password

### Public Client and Confidential Client Applications

Security tokens can be acquired by multiple types of applications. These applications tend to be separated into the following two categories. Each is ued with different libraries and objects.

* **Public client applications:** Are apps that run on devices or desktop computers or in a web browser. They're not trusted to safely keep application secrets, so they only access web APIs on behalf of the user. (They support only public client flows). Public clients can't hold configuration-time secrets, so they don't have client secrets.
* **Confidential client applications:** Are apps that run on servers (web apps, web API apps, or even service/daemon apps). They're considered difficult to access, and for that reason capable of keeping an application secret. Confidential clients can hold configuration-time secrets. Each instance of the client has a distinct configuration (including client ID and client secret).

### Initialize Client Apps

With MSAL.NET 3.x, the recommended way to instantiate an application is by using the application builders: `PublicClientApplicatinoBuilder` and `ConfidentialClientApplicationBuilder`. They offer a powerful mechanism to configure the application from the code, from a configuration file, or by mixing both approaches.

Before initializing an app, you first must register it so that your app can be integrated with the Microsoft identity platform. After registration, you may need the following information:

* The client ID (a string representing a GUID).
* The identity provider URL (naed the instance) and the sign-in audience for your application. These two parameters are collectively known as the authority.
* The tenant ID if you are writing a line of business application solely for your organization (also named single-tenant application).
* The application secret (client secret string) or certificate (of type X509Certificate2) if it's a confidential client app.
* For web apps, and sometimes for public client apps (in particularl when your app needs to use a broker), you'll have also set the redirectUri where the identity provider will contact back your application with the security tokens.

### Initializing Public and Confidential Client Applications from Code

The following code instantiates a public client application, signing-in users in the Microsoft Azure public cloud, with their work and school accounts, or their personal Microsoft accounts:

```cs
IPublicClientApplication app = PublicClientAplpicationBuilder
    .Create(clientId)
    .Build();
```

In the same way, the following code instantiates a confidential application (a Web app located at `https://myapp.azurewebsites.net`) handling tokesn from users in the Microsoft Azure public cloud, with their work and school accounts, or their personal Microsoft accounts. The application is identified with the identity provider by sharing a client secret:

```cs
string redirectUri = "https://myapp.azurewebsites.net";

IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
    .Create(clientId)
    .WithClientSecret(clientSecret)
    .WithRedirectUri(redirectUri)
    .Build();
```

### Builder Modifiers

In the code snippets using application builders, a number of `.With` methods can be applied as modifiers (for example, `.WithAuthority` and `.WithRedirectUri`).

* `.WithAuthority` modifier: The `.WithAuthority` modifier sets the application default authority to an Azure Active Directory authority, with the possibility of choosing the azure Cloud, the audience, the tenant (tenant ID or domain name), or providing directly the authority URI:

    ```cs
    IPublicClientApplication app = PublicClientApplicationBuilder
        .Create(client_id)
        .WithAuthority(AzureCloudInstance.AzurePublic, tenant_id)
        .Build();
    ```

* `.WithRedirectUri` modifier: The `.WithRedirectUri` modifier overrides the default redirect URI. In the case of public client applications, this will be useful for scenarios which require a broker:

    ```cs
    IPublicClientApplication app = PublicClientApplicationBuilder
        .Create(client_id)
        .WithAuthority(AzureCloudInstance.AzurePublic, tenant_id)
        .WithRedirectUri("http://localhost")
        .Build();
    ```

### Modifiers Common to Public and Confidential Client Applications

The table below lists some of the modifiers you can set on a public or confidential client:

Modifier | Description
---------|------------
`.WithAuthority()` | Sets the application default authority to an Azure Active Directory authority, with the possibility of choosing the Azure Cloud, the audience, the tenant (tenant ID or domain name), or providing directly the authority URI.
`.WithTenantId(string tenantId)` | Overrides the tenant ID, or the tenant description.
`.WithClientId(string)` | Overrides the client ID.
`.WithRedirectUri(string redirectUri)` | Overrides the default redirect URI. In the case of public client applications, this will be useful for scenarios requiring a broker.
`.WithComponent(string)` | Sets the name of the library using MSAL.NET (for telemetry reasons).
`.WithDebugLoggingCallback()` | If called, the application will call `debug.Write` simply enabling debugging traces.
`.WithLogging()` | If called, the application will call a callback with debugging traces.
`.WithTelemetry(TelemetryCallback callback)` | Sets the delegate used to send telemetry.

### Modifiers Specific to Confidential Client Applications

The modifiers you can set on a confidential client application builder are:

Modifier | Description
---------|------------
`.WithCertificate(X509Certificate2 certificate)` | sets the certificate identifying the application with Azure Active Directory.
`.WithClientSecret(string secret)` | Sets the client secret (app password) identifying the application with Azure Active Directory.