# Protect a Web API with the Microsoft Identity Platform

https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app

**Workflow Scripts So Far**

```bash
clientid=$(az ad app create \
    --display-name secure-api \
    --query appId \
    --output tsv)

az ad app update \
    --id $clientid \
    --identifier-uris api://$clientid 

# Create a new UUID
uuid=$(uuidgen)

# set the API object as a JSON object
api=$(echo '{
    "acceptMappedClaims": null,
    "knownClientApplications": [],
    "oauth2PermissionScopes": [{
        "adminConsentDescription": "Access Secure API",
        "adminConsentDisplayName": "Access SecureApi",
        "id": "'$uuid'",
        "isEnabled": true,
        "type": "User",
        "userConsentDescription": "Access Sescure API",
        "userConsentDisplayName": "Access SecureApi",
        "value": "access_as_user"
    }],
    "preAuthorizedApplications": [],
    "requestedAccessTokenVersion": 2
}' | jq .)

# Update app registration with App ID URL and api object
az ad app update \
    --id $clientid \
    --identifier-uris api://$clientid \
    --set api="$api"
```

**Helpful Scripts**

```bash
# set the ID of the app registration as a variable
objectid=$(az ad app show \
    --id $clientid \
    --query id \
    --output tsv)

# get the app registration object with az rest
az rest \
    --method GET \
    --headers "Content-Type=application/json" \
    --uri https://graph.microsoft.com/v1.0/applications/$objectid/
```