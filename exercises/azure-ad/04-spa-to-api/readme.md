# Spa to API Auth

https://github.com/Azure-Samples/ms-identity-javascript-angular-tutorial/tree/main/3-Authorization-II/1-call-api

## Create and Configure API App Registration

```bash
appId=$(az ad app create \
    --display-name stack-api \
    --sign-in-audience AzureADMyOrg \
    --query appId \
    --output tsv)

objectId=$(az ad app show \
    --id $appId \
    --query id \
    --output tsv)

# if uuidgen is not available
sudo apt install uuid-runtime

readId=$(uuidgen)

# Delegated Permissions

read=$(echo '{
    "adminConsentDescription": "Allow the API to read signed-in users Todolist items.",
    "adminConsentDisplayName": "Read Todolist items.",
    "id": "'$readId'",
    "isEnabled": true,
    "type": "User",
    "userConsentDescription": "Allow the API to read Todolist items on your behalf.",
    "userConsentDisplayName": "Read Todolist items as yourself.",
    "value": "TodoList.Read"
}')

readwriteId=$(uuidgen)

readwrite=$(echo '{
    "adminConsentDescription": "Allow the API to read and write signed-in users Todolist items.",
    "adminConsentDisplayName": "Read and Write Todolist items.",
    "id": "'$readwriteId'",
    "isEnabled": true,
    "type": "User",
    "userConsentDescription": "Allow the API to read and write Todolist items on your behalf.",
    "userConsentDisplayName": "Read and Write Todolist items as yourself.",
    "value": "TodoList.ReadWrite"
}')
```

```bash
api=$(echo '{
    "acceptMappedClaims": null,
    "knownClientApplications": [],
    "oauth2PermissionScopes": [
        '$read',
        '$readwrite'
    ],
    "preAuthorizedApplications": [],
    "requestedAccessTokenVersion": 2
}' | jq .)
```

```bash
az ad app update \
    --id $appId \
    --identifier-uris api://$appId \
    --set api="$api"
```