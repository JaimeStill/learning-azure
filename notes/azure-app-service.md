# Azure App Service

> [Azure CLI Reference](https://learn.microsoft.com/en-us/cli/azure/service-page/azure%20app%20service?view=azure-cli-latest)

Azure App Service is a fully managed web application hosing platform. This platform as a service (PaaS) offered by Azure allows you to focus on designing and building your app while Azure takes care of the infrastructure to run and scale your applications.

## Deployment Slots

Using the Azure portal, you can easily add **deployment slots** to an App Service web app. For instance, you can create a **staging** deployment slot where you can push your code to test on Azure. Once you're happy with your code, you can easily **swap** the staging deployment slot with the production slot.

## Continuous Integration / Deployment Support

The Azure portal provides out-of-the-box continuous integration and deployment with Azure DevOps, GitHub, BitBucket, FTP, or a local Git repository on your development machine. Connect your web app with any of the above sources, and App Service will do the rest for you by automatically syncing your code and any future changes on the code into the web app. Furthermore, with Azure DevOps, you can define your own build and release process that compiles your source code, run the tests, builds a release, and finally deploys the release into your web app every time you commit the code. All that happens implicitly without any need for you to intervene.

## Built-in Auto Scale Support

The ability to scale up / down or scale out is baked into the web app. Depending on the web app's usage, you can scale your app up / down by increasing / decreasing the resources of the underlying machine that is hosting your web app. REsources can be number of cores or the amount of RAM available.

Scaling out, on the other hand, is the ability to increase the number of machine instances that are running your web app.

## Creating a Web App

> [`az webapp create`](https://learn.microsoft.com/en-us/cli/azure/webapp?view=azure-cli-latest#az-webapp-create) for CLI.  
> Must first have an AppService plan, which can be created with [`az appservice plan create`](https://learn.microsoft.com/en-us/cli/azure/appservice/plan?view=azure-cli-latest#az-appservice-plan-create)  

When you're ready to run a web app on Azure, you visit the Azur portal and create a **Web App** resource. Creating a web app allocates a set of hosting resources in App Service, which you can use to host any web-based application that is supported by Azure, whether it be ASP.NET Core, Node.js, Java, Python, etc.

The Azure portal provides a wizard to create a web app. The wizard requires the following fields:

**Field** | **Description**
----------|----------------
**Subscription** | A valid and active Azure subscription.
**Resource group** | A valid resource group.
**App name** | The name of the web app. This name becomes part of the app's URL, so it must be unique among all Azure App Service web apps.
**Publish** | You can deploy your application to App Service as **code** or as a ready-to-run **Docker image**. Selecting **Docker imgae** will activate the Docker tab of the wizard, where you'll provide information about the Docker registry from which App Service will retrieve your image.
**Runtime stack** | If you choose to deploy your application as code, App Service needs to know what runtime your application uses (examples include Node.js, Python, Java, and .NET). If you deploy your application as a Docker image, you won't need to choose a runtime stack, since your image will include it.
**Operating System** | App Service can host applications on **Windows** or **Linux** servers. See below for additional information.
**Region** | The Azure region from which your application will be served.
**App Service Plan** | See below information about App Service plans.

## Operating Systems

If you're deploying your app as code, many of the available runtime stacks are limited to one operating system or the other. After choosing a runtime stack, the toggle will indicate whether or not you have a choice of operating system. If your target runtime stack is available on both operating systems, select the one that you use to develop and test your application.

If your application is packaged as a Docker image, choose the operating system on which your image is designed to run.

Selecting **Windows** activates the Monitoring tab, where you have teh option to enable **Application Insights**. Enabling this feature will configure your app to automatically send detailed performance telemetry to the Application Insights monitoring service wtihout requiring any changes to your code. You can use Application Insights from Linux-hosted apps as well, but this turnkey, no-code option is only available on Windows.

## App Service Plans

An **App Service** plan is a set of virtual server resources that run App Service apps. A plan's **size** (sometimes referred to as its **sku** or **pricing tier**) determines the performance characteristics of the virtual servers that run the apps assigned to the plan, as well as the App Service features to which those apps have access. Every APp Service web app you create must be assigned toa  single App Service plan that runs it.

A single App Service plan caan host multiple App Service web apps. In most cases, the number of apps you can run on a single plan will be limited to the performance characteristics of the apps and the resource limitations of the plan.

App Service plans are the unit of billing for App Service. the size of each App Service plan in your subscription, in addition to the bandwidth resources used by the apps deployed to those plans, determines the price that you pay. The number of web apps deployed to your App Service plans has no effect on your bill.

You can use any of the available Azure management tools to create an App Service plan. When you create a web app via the Azure portal, the wizard will help you create a new plan at the same time if you don't already have one.

## Automated Deployment

Automated deployment, or continuous integration, is a process used to push out new features and bug fixes in a fast and repetitive pattern with minimal impact on end users.

Azure supports automated deployment directly from several sources:

* **Azure DevOps**: You can push you code to Azure DevOps, build your code in the cloud, run the tests, generate a release from the code, and finally, push your code to an Azure Web App.
* **GitHub**: Azure supports automated deployment directly from GitHub. When you connect your GitHub repository to Azure for automated deployment, any changes you push to your production branch on GitHub will automatically be deployed for you.
* **Bitbucket**: With its similarities to GitHub, you can configure an automated deployment with Bitbucket.
* **OneDrive**: OneDrive is Microsoft's cloud-based storage. You must have a Microsoft Account liked to a OneDrive account to deploy to Azure.
* **Dropbox**: Azure supports deployment from Dropbox, which is a popular cloud-based storge system that is similar to OneDrive.

## Manual Deployment

There are a few options that you can use to manually push your code to Azure:

* **Git**: App Service web apps feature a Git URL that you can add as a remote repository. Pushing to the remote repository will deploy your app.
* **az webapp up**: `webapp up` is a feature of the `az` command-line interface that packages your app and deploys it. Unlike other deployment methods, `az webapp up` can create a new App Service web app for you if you haven't already created one.
* **ZIP deploy**: You can use `az webapp deployment source config-zip` to send a ZIP of your application files to App Service. YOu can also access ZIP deploy via basic HTTP utilities such as `curl`.
* **WAR deploy**: WAR deploy is an App Service deployment mechanism designed for deploying Java web applications using WAR packages. You can access WAR deploy using Kudu HTTP API located at `http://<your-app-name>.scm.azurewebsites.net/api/wardeploy`. If that fails, try : `https://<your-app-name>.scm.azurewebsites.net/api/wardeploy`.
* **Visual Studio**: Visual Studio features an App Service deployment wizard that can walk you through the deployment process.
* **FTP/S**: FTP or FTPS is a traditional way of pushing your code to many hosting environment, including App Service.