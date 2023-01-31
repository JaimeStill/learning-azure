# App Package Management Using Helm

You use an Azure Kubernetes Service (AKS) cluster to host the drone tracking solution. The DevOps team uses standard declarative YAML files to deploy various services in the solution. In the current deployment workflow, the development teams create the deployment files for each application. The team is reviewing Helm as an alternative to simplify the management of cloud-native application deployments.

In this exercise, you'll install an AKS cluster for the team test deployments using HELM. You'll use a script to create and configure your Azure Kubernetes Service (AKS) cluster.

The script does the following configuration setps:

* Select the subscription to use with this module's exercises and set it as the default subscription for all deployed resources.
* Create an Azure Kubernetes Service cluster by using the Azure CLI.
* Configures a default Kubernetes NGINX Ingress controller and LoadBalancer.
* Clone the example web app and Helm chart from a GitHub repository.
* Captures all configuration values in `/home/*user*/clouddrive/mslearn-aks/create-aks-exports.txt` for easy reference.
* [Optional] Create an Azure Container Registry by using the Azure CLI.
* [Optional] Configure an AKS cluster to authenticate to an Azure Container.
* [Optional] Installs version 3.1.302 of the .NET SDK, alongside the default SDK version of Cloud Shell.

> Some of the above items are optional installation steps that are disabled in this exercise using command arguments.

## Environment Setup

You'll use `bash` deployment script that uses the following parameters to deploy a new cluster:

Parameter | Description
----------|------------
`-s` | Identifies the subscription ID to use.
`-n` | Identifies the AKS cluster name and resource group used to create the cluster in the context of this module.
`--use-acr` | Allows the script to configure a default ACR with the name `mslearn-kubernetes-acr`. Default value is `false`.
`--install-dot-net` | Allows the script to install the .NET SDK. Default value is set to `false`.

1. Run the command below in the Azure Cloud Shell:

    ```bash
    SubscriptionId=$(az account list --query '[0].id' -o tsv)
    . <(wget -q -O - https://raw.githubusercontent.com/MicrosoftDocs/mslearn-aks/main/infrastructure/setup/setup.sh) -s $SubscriptionId -n learn-helm-deploy-aks --use-acr false --install-dot-net false
    ```

### How to Manage an Unexpected Failure

Non-blocking warnings are expected in the deployment process. If an unexpected exception occurs, you can reset any changes made by the script by running teh following command:

    ```bash
    cd ~ && \
        rm -rf ~/clouddrive/mslearn-aks && \
        az group delete --name learn-helm-deploy-aks-rg --yes
    ```

## Install a Helm Chart

Helm charts make it easy to install pre-configured cloud-native apps on a Kubernetes cluster. The DevOps team is already familiar with the installation steps to install apps using manifest files and kubectl. The team decides to install an ASP.NET Core Helm chart to test the Helm installation process.

1. Add the Azure Marketplace Helm repository to the Helm client. This repository gives you access to a number of pre-configured Helm charts:

    ```bash
    helm repo add azure-marketplace https://marketplace.azurecr.io/helm/v1/repo
    ```

    Run the helm repo list command to confirm the newly added repository:

    ```bash
    helm repo list
    ```

    The command should return a result similar to the following output:

    NAME | URL
    -----|----
    azure-marketplace | https://marketplace.azurecr.io/helm/v1/repo

2. Run the helm search repo command to search for the `azure-marketplace/aspnet-core` chart:

    ```bash
    helm search repo aspnet
    ```

    Here is an example of what the listing may look like:

    NAME | CHART VERSION | APP VERSION | DESCRIPTION
    -----|---------------|-------------|------------
    azure-marketplace/aspnet-core | 1.3.18 | 3.1.19 | ASP.NET Core is an open-source framework create...

## Deploy a Helm Chart

1. Deploy the ASP.NET Core Helm chart by using the `helm install` command:

    ```bash
    helm install aspnet-webapp azure-marketplace/aspnet-core
    ```

    The command should return a result similar to the following output:

    ```
    NAME: aspnet-webapp
    LAST DEPLOYED: Mon Oct 11 17:12:43 2021
    NAMESPACE: default
    STATUS: deployed
    REVISION: 1
    TEST SUITE: None
    NOTES:
    ** Please be patient while the chart is being deployed **

    ASP.NET Core can be accessed through the following DNS name from within your cluster:

        aspnet-webapp-aspnet-core.default.svc.cluster.local (port 80)

    To access ASP.NET Core from outside the cluster execute the following commands:

    1. Get the ASP.NET Core URL by running these commands:

        export SERVICE_PORT=$(kubectl get --namespace default -o jsonpath="{.spec.ports[0].port}" services aspnet-webapp-aspnet-core)
        kubectl port-forward --namespace default svc/aspnet-webapp-aspnet-core ${SERVICE_PORT}:${SERVICE_PORT} &
        echo "http://127.0.0.1:${SERVICE_PORT}"

    2. Access ASP.NET Core using the obtained URL.
    ```

    The above output is generated from the `templates/Notes.txt` file. The information displayed from the `Notes.txt` file is generated based on a template defined in the file and values from the `values.yaml` file.

    For example, notice how the name of the chart, `aspnet-webapp`, is used to create the DNS name, `aspnet-webapp-aspnet-core.default.svc.cluster.local`, for the web app. You'll also notice the notes displays information to access the app via a service. The default release doesn't include an Ingress as part of the install.

2. Helm allows you to query all the installed releases on thet cluseter. Use the `helm list` command to list all Helm releases:

    ```bash
    helm list
    ```

    The command should return a result similar to the following output:

    NAME | NAMESACE | REVISION | UPDATED | STATUS | CHART | APP VERSION
    -----|----------|----------|---------|--------|-------|------------
    aspnet-webapp | default | 1 | 2021-10-11 17:12:43.50734334 +0000 UTC | deployed | aspnet-core-1.3.18 | 3.1.19

    Notice the name of the release and its revision number. The name of the release is important, as you'll use the name to reference the release. The revision number increments each time you make a change to a release.

3. Helm allows you to fetch manifest information related to each release by using the `helm get manifest` command:

    ```bash
    helm get manifest aspnet-webapp
    ```

    The command should return a result similar to the following output:

    ``` yml
    ---
    # Source: aspnet-core/templates/serviceaccount.yaml
    apiVersion: v1
    kind: ServiceAccount
    metadata:
    name: aspnet-webapp-aspnet-core
    ...
    ---
    # Source: aspnet-core/templates/svc.yaml
    apiVersion: v1
    kind: Service
    metadata:
    name: aspnet-webapp-aspnet-core
    ...
    ---
    # Source: aspnet-core/templates/deployment.yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
    name: aspnet-webapp-aspnet-core
    ...
    ```

    Notice the three YAML files from the templates folder in the chart:

    * ServiceAccount
    * Service
    * Deployment

    These files are rendered based on the combination of the chart's available templates and the values available in the `values.yaml` file.

4. Validate the pod is deployed by running the `kubectl get pods` command:

    ```bash
    kubectl get pods -o wide -w
    ```

    The command should return a result similar to the following output:

    NAME | READY | STATUS | RESTARTS | AGE | IP | NODE | NOMINATED NODE | READINESS GATES
    -----|-------|--------|----------|-----|----|------|----------------|----------------
    aspnet-webapp-aspnet-core-7cb658b89d-9fxwj | 1/1 | Running   0 | 5m16s | 10.244.0.10 | aks-nodepool1-41833800-vmss000000 | \<none\> | \<none\>

## Delete a Helm Release

1. Delete the Helm release by using the helm delete command:

    ```bash
    helm delete aspnet-webapp
    ```

    The command should return a result similar to the following:

    ```
    release "aspnet-webapp" uninstalled
    ```

## Install a Helm Chart with Set Values

You may override values for a Helm chart by passing either a value parameter or your own `values.yaml` file.

1. For now, use the following command to see how to update a value using the `--set` parameter.

    Run `helm install` with the `--set` parameter to set the `replicaCount` of the deployment template to five replicas:

    ```bash
    helm install --set replicaCount=5 aspnet-webapp azure-marketplace/aspnet-core
    ```

    Validate that five pod replicas are deployed by running the `kubectl get pods` command:

    ```bash
    kubectl get pods -o wide -w
    ```

    The command should return a result similar to the following:

    NAME | READY | STATUS | RESTARTS | AGE | IP | NODE | NOMINATED NODE | READINESS GATES
    -----|-------|--------|----------|-----|----|------|----------------|----------------
    aspnet-webapp-aspnet-core-7cb658b89d-2q96n | 0/1 | Init:0/2 | 0 | 14s | 10.244.0.14 | aks-nodepool1-41833800-vmss000000 | \<none\> | \<none\>
    aspnet-webapp-aspnet-core-7cb658b89d-469f2 | 0/1 | Init:0/2 | 0 | 14s | 10.244.0.15 | aks-nodepool1-41833800-vmss000000 | \<none\> | \<none\>
    aspnet-webapp-aspnet-core-7cb658b89d-bl9lc | 0/1 | Init:0/2 | 0 | 14s | 10.244.0.12 | aks-nodepool1-41833800-vmss000000 | \<none\> | \<none\>
    aspnet-webapp-aspnet-core-7cb658b89d-tlv7r | 0/1 | Init:0/2 | 0 | 14s | 10.244.0.13 | aks-nodepool1-41833800-vmss000000 | \<none\> | \<none\>
    aspnet-webapp-aspnet-core-7cb658b89d-zgsdp | 0/1 | Init:0/2 | 0 | 14s | 10.244.0.16 | aks-nodepool1-41833800-vmss000000 | \<none\> | \<none\>

2. Delete the Helm chart by using the `helm delete` command. This command will delete the release and all replicas of the workload:

    ```bash
    helm delete aspnet-webapp
    ```