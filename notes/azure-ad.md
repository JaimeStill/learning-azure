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