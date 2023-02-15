# SPA Authentication

https://learn.microsoft.com/en-us/azure/active-directory/develop/tutorial-v2-angular-auth-code

[ng-auth.webm](https://user-images.githubusercontent.com/14102723/218915009-7c0ddd91-3546-4783-bfd6-ba22d6f19ab6.webm)

# spa-auth
Workout SPA access to an Azure AD restricted API

**Project Generation**  

```bash
npm i -g npm @angular/cli

ng new msal-angular-tutorial \
    --directory=src/app \
    --routing=true \
    --style=css \
    --strict=false

npm i @angular/material \
    @angular/cdk \
    @azure/msal-browser \
    @azure/msal-angular

ng generate component home
ng generate component profile
```

**App Registration and Configuration**  

```bash
appId=$(az ad app create \
    --display-name spa-api \
    --query appId \
    --output tsv)

objectId=$(az ad app show \
    --id $appId \
    --query id \
    --output tsv)

# use to verify structure of spa configuration
az rest \
    --method GET \
    --headers "Content-Type=application/json" \
    --uri https://graph.microsoft.com/v1.0/applications/$objectId/

az rest \
    --method PATCH \
    --uri https://graph.microsoft.com/v1.0/applications/$objectId/ \
    --headers 'Content-Type=application/json' \
    --body '{"spa":{"redirectUris":["http://localhost:4200"]}}'

# if uuidgen is not available
sudo apt install uuid-runtime

az ad app update \
    --id $appId \
    --identifier-uris api://$appId
```

Follow tutorial instructions from [Configure the Application](https://learn.microsoft.com/en-us/azure/active-directory/develop/tutorial-v2-angular-auth-code#configure-the-application)