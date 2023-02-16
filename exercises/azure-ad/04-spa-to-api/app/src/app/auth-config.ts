import {
    BrowserCacheLocation,
    Configuration,
    LogLevel
} from '@azure/msal-browser';

const isIE = window.navigator.userAgent.indexOf("MSIE ") > -1
    || window.navigator.userAgent.indexOf("Trident/") > -1;

export const msalConfig: Configuration = {
    auth: {
        clientId: '11e01888-a357-4f91-a1be-35105c4c507e', // SPA App ID
        authority: 'https://login.microsoft.com/64819121-d17e-4216-a81e-fa8528635fb8', // AZ Tenant ID
        redirectUri: '/auth',
        postLogoutRedirectUri: '/',
        clientCapabilities: ['CP1'] // client can handle claim challenges
    },
    cache: {
        cacheLocation: BrowserCacheLocation.LocalStorage,
        storeAuthStateInCookie: isIE
    },
    system: {
        loggerOptions: {
            loggerCallback(_: LogLevel, message: string) {
                console.log(message);
            },
            logLevel: LogLevel.Verbose,
            piiLoggingEnabled: false
        }
    }
}

export const protectedResources = {
    apiTodoList: {
        endpoint: "https://localhost:44351/api/todolist",
        scopes: {
            /*
                API Identifier URI + App Role
                az ad app show \
                    --id {API App ID} \
                    --query "identifierUris[0]" \
                    --output tsv
            */
            read: ["api://0512e79b-5849-4ead-9c01-03e90ceb8270/TodoList.Read"],
            write: ["api://0512e79b-5849-4ead-9c01-03e90ceb8270/TodoList.ReadWrite"]
        }
    }
}

export const loginRequest = {
    scopes: []
}