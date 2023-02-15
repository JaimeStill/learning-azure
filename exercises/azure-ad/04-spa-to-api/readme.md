# Spa to API Auth

https://github.com/Azure-Samples/ms-identity-javascript-angular-tutorial/tree/main/3-Authorization-II/1-call-api

## Create and Configure API App Registration

```bash
apiAppId=$(az ad app create \
    --display-name stack-api \
    --sign-in-audience AzureADMyOrg \
    --query appId \
    --output tsv)

apiObjectId=$(az ad app show \
    --id $apiAppId \
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

readWriteId=$(uuidgen)

readWrite=$(echo '{
    "adminConsentDescription": "Allow the API to read and write signed-in users Todolist items.",
    "adminConsentDisplayName": "Read and Write Todolist items.",
    "id": "'$readWriteId'",
    "isEnabled": true,
    "type": "User",
    "userConsentDescription": "Allow the API to read and write Todolist items on your behalf.",
    "userConsentDisplayName": "Read and Write Todolist items as yourself.",
    "value": "TodoList.ReadWrite"
}')

api=$(echo '"api": {
    "oauth2PermissionScopes": [
        '$read',
        '$readWrite'
    ]
}')

# App Roles

appReadId=$(uuidgen)

appRead=$(echo '{
    "allowedMemberTypes": [
        "Application"
    ],
    "description": "Allow this application to read every users Todo list items.",
    "displayName": "TodoList.Read.All",
    "id": "'$appReadId'",
    "isEnabled": true,
    "origin": "Application",
    "value": "TodoList.Read.All"
}')

appReadWriteId=$(uuidgen)

appReadWrite=$(echo '{
    "allowedMemberTypes": [
        "Application"
    ],
    "description": "Allow this application to read and write every users Todo list items.",
    "displayName": "TodoList.ReadWrite.All",
    "id": "'$appReadWriteId'",
    "isEnabled": true,
    "origin": "Application",
    "value": "TodoList.ReadWrite.All"
}')

appRoles=$(echo '"appRoles": [
    '$appRead',
    '$appReadWrite'
]')

# Identifier URI

identifierUris=$(echo '"identifierUris": [
    "api://'$apiAppId'"
]')

# Optional Claims

optionalClaims=$(echo '"optionalClaims": {
    "accessToken": [
        {
            "additionalProperties": [],
            "essential": false,
            "name": "idtyp",
            "source": null
        }
    ]
}')

# Compose Body

body=$(echo '{
    '$api',
    '$appRoles',
    '$identifierUris',
    '$optionalClaims'
}' | jq .)

az rest \
    --method PATCH \
    --uri https://graph.microsoft.com/v1.0/applications/$apiObjectId/ \
    --headers 'Content-Type=application/json' \
    --body "$body"
```

## Register and Configure SPA App Registration

```bash
spaAppId=$(az ad app create \
    --display-name stack-spa \
    --sign-in-audience AzureADMyOrg \
    --query appId \
    --output tsv)

spaObjectId=$(az ad app show \
    --id $spaAppId \
    --query id \
    --output tsv)

# API Permissions

requiredResourceAccess=$(echo '"requiredResourceAccess": [
    {
        "resourceAccess": [
            {
                "id": "'$appReadId'",
                "type": "Scope"
            },
            {
                "id": "'$appReadWriteId'",
                "type": "Scope"
            }
        ],
        "resourceAppId": "'$apiAppId'"
    }
]')

# Redirect URIs

spa=$(echo '"spa": {
    "redirectUris": [
        "http://localhost:4200/",
        "http://localhost:4200/auth"
    ]
}')

# Compose Body

body=$(echo '{
    '$requiredResourceAccess',
    '$spa'
}' | jq .)

az rest \
    --method PATCH \
    --uri https://graph.microsoft.com/v1.0/applications/$spaObjectId/ \
    --headers 'Content-Type=application/json' \
    --body "$body"
```