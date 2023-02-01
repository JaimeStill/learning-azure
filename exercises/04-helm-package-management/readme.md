# App Package Management Using Helm

> The following exercise should be run from an [Azure Cloud Shell](https://portal.azure.com/#cloudshell/) with the Bash profile.

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

Non-blocking warnings are expected in the deployment process. If an unexpected exception occurs, you can reset any changes made by the script by running the following command:

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

## Manage a Helm Release

Helm charts make it easy to install, upgrade, roll back, and delete an application on a Kubernetes cluster. Earlier, the team was successful in installing the pre-configured Helm chart from the Azure Marketplace Helm repository.

To test the management of release upgrades and rollbacks, the team decides to simulate the experience by installing a basic .NET Core Blazor Server application.

## Review the Helm Chart Folder Structure

This exercise uses the `aspnet-core` Helm chart you installed earlier from the Azure Marketplace as a foundation to install a .NET Core Blazor Server app. The source code to the application is already downloaded as part of the AKS cluster creation exercise and available in the `~/mslearn-aks/modules/learn-helm-deploy-aks/src/drone-webapp` folder.

You can find a cache copy of the Helm chart in the `$HOME/.cache/helm/repository` folder.

1. List the contents of the `$HOME/.cache/helm/repository` folder to locate the `aspnet-core-1.3.18.tgz` file.

    ```bash
    ls $HOME/.cache/helm/repository -l
    ```

    The command output should return a result similar to the following output:

    ```
    -rw-r--r-- 1 user user   30621 Oct 11 17:25 aspnet-core-1.3.18.tgz
    -rw-r--r-- 1 user user    1391 Oct 11 17:04 azure-marketplace-charts.txt
    -rw-r--r-- 1 user user 4834112 Oct 11 17:04 azure-marketplace-index.yaml
    ```

    All Helm charts installed from repositories are cached in this folder. If you're interested in inspecting or modifying the contents of a chart, you can extract the zipped package from the cache folder and install the chart as a local chart.

    For this exercise, the chart is already unpacked and available in the `~/mslearn-aks/modules/learn-helm-deploy-aks/src/drone-webapp-chart` folder.

2. Inspect the existing Helm chart by recursively listing all contents of the `drone-webapp-chart` folder:

    ```bash
    cd ~/clouddrive/mslearn-aks/modules/learn-helm-deploy-aks/src
    find drone-webapp-chart/ -print
    ```

    The command should return a result similar to the following output:

    ```
    drone-webapp-chart/
    drone-webapp-chart/.helmignore
    drone-webapp-chart/Chart.yaml
    drone-webapp-chart/templates
    drone-webapp-chart/templates/deployment.yaml
    drone-webapp-chart/templates/extra-list.yaml
    drone-webapp-chart/templates/health-ingress.yaml
    drone-webapp-chart/templates/hpa.yaml
    drone-webapp-chart/templates/ingress.yaml
    drone-webapp-chart/templates/NOTES.txt
    drone-webapp-chart/templates/pdb.yaml
    drone-webapp-chart/templates/service.yaml
    drone-webapp-chart/templates/serviceaccount.yaml
    drone-webapp-chart/templates/tls-secret.yaml
    drone-webapp-chart/templates/_helpers.tpl
    drone-webapp-chart/values.yaml
    ```

    Notice the following components in this chart:

    * a `Chart.yaml` file
    * a `values.yaml` file
    * a number of templates in the `templates/` folder

3. Open `drone-webapp-chart/Chart.yaml` to review the chart's dependencies:

    ```bash
    code drone-webapp-chart/Chart.yaml
    ```

    The editor will show the following:

    ```yml
    apiVersion: v2
    appVersion: 0.0.1
    description: ASP.NET Core is an open-source framework created by Microsoft for building
    cloud-enabled, modern applications.
    home: https://dotnet.microsoft.com/apps/aspnet
    icon: https://bitnami.com/assets/stacks/aspnet-core/img/aspnet-core-stack-220x234.png
    keywords:
    - asp.net
    - dotnet
    maintainers:
    - email: containers@bitnami.com
    - name: Bitnami
    name: drone-tracker
    sources:
    - https://github.com/bitnami/bitnami-docker-aspnet-core
    annotations:
      category: DeveloperTools
    version: 1.3.18
    dependencies:
      - name: common
        version: 1.x.x
        repository: https://marketplace.azurecr.io/helm/v1/repo
        tags:
          - bitnami-common
    ```

    Notice the dependencies section at the bottom of the file. This information shows you that there's a subchart to the main chart.

4. Run the `helm dependency build` command to download and update all chart dependencies:

    ```bash
    helm dependency build ./drone-webapp-chart
    ```

    The command should return a result similar to the following:

    ```
    Hang tight while we grab the latest from your chart repositories...
    ...Successfully got an update from the "azure-marketplace" chart repository
    ...Successfully got an update from the "bitnami" chart repository
    Update Complete. ⎈Happy Helming!⎈
    Saving 1 charts
    Downloading common from repo https://marketplace.azurecr.io/helm/v1/repo
    Deleting outdated charts
    ```

5. Review the files in the `drone-webapp-chart` folder to see the contents of the Helm download:

    ```bash
    find drone-webapp-chart/ -print
    ```

    The command should return a result similar to the following output. The output is shortened for brevity:

    ```
    drone-webapp-chart/
    ...
    drone-webapp-chart/Chart.yaml
    drone-webapp-chart/charts/common-1.10.0.tgz
    drone-webapp-chart/templates/deployment.yaml
    drone-webapp-chart/templates/...
    drone-webapp-chart/values.yaml
    ```

    An updated subchart named `common` is now available in the `charts/` folder. You can extract the contents of the `common-1.10.0.tgz` package if you're interested in its contents. However, unpacking the file isn't required to complete the installation of the chart.

    Here is the command to unpack the file and list the folder contents:

    ```bash
    gzip -dc ./drone-webapp-chart/charts/common-1.10.0.tgz | tar -xf - -C ./drone-webapp-chart/charts/
    find drone-webapp-chart/ -print
    ```

    The command should return a result similar to the following:

    ```
    drone-webapp-chart/
    drone-webapp-chart/Chart.yaml
    ...
    drone-webapp-chart/charts/common
    drone-webapp-chart/charts/common/.helmignore
    drone-webapp-chart/charts/common/Chart.yaml
    drone-webapp-chart/charts/common/README.md
    drone-webapp-chart/charts/common/templates
    drone-webapp-chart/charts/common/templates/_affinities.tpl
    drone-webapp-chart/charts/common/templates/_capabilities.tpl
    drone-webapp-chart/charts/common/templates/_errors.tpl
    drone-webapp-chart/charts/common/templates/validations/...
    drone-webapp-chart/charts/common/templates/...
    drone-webapp-chart/charts/common/values.yaml
    drone-webapp-chart/charts/common-1.10.0.tgz
    drone-webapp-chart/templates/deployment.yaml
    ...
    drone-webapp-chart/values.yaml
    ```

    Notice how the subchart also replicates the standard chart structure. It has a `Chart.yaml` file, a `values.yaml` file, and a `templates/` folder. However, there's a difference. All of the files in the templates folder end with the `.tpl` extension. These files contain custom functions used in the main chart.

    A chart dependency isn't limited to other applications. You may decide to reuse template logic across your charts and create a dependency specific to managing this logic as a chart dependency.

## Review the Chart `values.yaml` File

The `aspnet-core` Helm chart has an extensive set of configuration options available to manage the overall deployment of a cloud-native web app. It's helpful to review the contents of `values.yaml` to get an overview of the deployment.

1. Open `values.yaml`:

    ```bash
    code ./drone-webapp-chart/values.yaml
    ```

2. Search for the `image` value to see which image is used for the web app:

    ```yml
    ...
    image:
    registry: docker.io
    repository: bitnami/aspnet-core
    tag: 3.1.19-debian-10-r0
    pullPolicy: IfNotPresent
    ...
    ```

    Notice the `bitnami/aspnet-core` Docker image. This image is the app's base image and contains an installation of [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet).

3. Search for the `appFromExternalRepo` value. You'll use these values to generate the `deployment.yaml` manifest file. Here is an extract of the `appFromExternalRepo` section in the `values.yaml` file:

    ```yml
    appFromExternalRepo:
      enabled: true
      clone:
        image:
          registry: docker.io
          repository: bitnami/git
          tag: 2.33.0-debian-10-r28
          pullPolicy: IfNotPresent
          pullSecrets: []
        repository: https://github.com/MicrosoftDocs/mslearn-aks.git
        revision: main
        extraVolumeMounts: []
    publish:
      image:
        registry: docker.io
        repository: bitnami/dotnet-sdk
        tag: 3.1.412-debian-10-r33
        pullPolicy: IfNotPresent
        pullSecrets: []
      subFolder: modules/learn-helm-deploy-aks/src/drone-webapp
      extraFlags: []
      startCommand: ["dotnet", "drone-webapp.dll"]
    ```

    There are several items specified in this section to note:

    * The `bitnami/git` image
    * The `bitnami/dotnet-sdk` image
    * The GitHub repo `https://github.com/MicrosoftDocs/mslearn-aks.git`
    * The app source code repository subfolder `modules/learn-helm-deploy-aks/src/drone-webapp`

    Each of these values is used in the `templates/deployment.yaml` file.

4. Next, search for the `ingress` value in the `values.yaml` file. You'll use these values to generate the `ingress.yaml` manifest file. Here is an extract of the `ingress` section in the `values.yaml` file:

    ```yml
    ingress:
      enabled: true
      pathType: ImplementationSpecific
      hostname: aspnet-core.local
      path: /
      annotations: {}
      tls: false
      secrets: []
    ```

    This section allows you to configure many aspects of the final Ingress file. For this exercise, you're only going to use the `ingress.enabled` value.

## Review the `deployment.yaml` and `ingress.yaml` Template Files

The values in the `values.yaml` file are combined with the templates in the chart's `templates/` folder to create the final manifest files. It's helpful to review the contents of the `deployment.yaml` and the `ingress.yaml` files to get an overview of the deployment.

1. Open `templates/deployment.yaml`:

    ```bash
    code ./drone-webapp-chart/templates/deployment.yaml
    ```

    Here is an extract of the files contents:

    ```yml
    apiVersion: {{ include "common.capabilities.deployment.apiVersion" . }}
    kind: Deployment
    metadata:
      ...
    spec:
      replicas: {{ .Values.replicaCount }}
      ...
        initContainers:
          {{- if .Values.appFromExternalRepo.enabled }}
          - name: clone-repository
            image: {{ include "aspnet-core.git.image" . }}
            imagePullPolicy: {{ .Values.appFromExternalRepo.clone.image.pullPolicy | quote }}
            command:
              - /bin/bash
              - -ec
              - |
                [[ -f "/opt/bitnami/scripts/git/entrypoint.sh" ]] && source "/opt/bitnami/scripts/git/entrypoint.sh"
                git clone {{ .Values.appFromExternalRepo.clone.repository }} --branch {{ .Values.appFromExternalRepo.clone.revision }} /repo
            volumeMounts:
              - name: repo
                mountPath: /repo
          - name: dotnet-publish
            image: {{ include "aspnet-core.sdk.image" . }}
            imagePullPolicy: {{ .Values.appFromExternalRepo.publish.image.pullPolicy | quote }}
            workingDir: /repo
            command:
              - /bin/bash
              - -ec
              - |
                cd {{ .Values.appFromExternalRepo.publish.subFolder }}
                dotnet publish -o /app {{ .Values.appFromExternalRepo.publish.extraFlags | join " " }}
            volumeMounts:
              - name: app
                mountPath: /app
              - name: repo
                mountPath: /repo
          {{- end }}
          {{- if .Values.initContainers }}
          {{- include "common.tplvalues.render" (dict "value" .Values.initContainers "context" $) | nindent 8 }}
          {{- end }}
        {{- end }}
        ...
    ```

    Notice that this file has the structure of a Deployment manifest, but the final information that goes into the file is dependent on values from the `values.yaml` file and template control flow logic.

    For example, the `spec.replicas` for this deployment is defined by `{{ .Values.replicaCount }}` and how container initialization is determined by `.Values.appFromExternalRepo.enabled` value in the `{{ - if .Values.appFromExternalRepo.enabled }}` statement.

    The rest of this section then makes use of the Git image and repository values to clone the repo and build the web app.

2. Open `templates/ingress.yaml`:

    ```bash
    code ./drone-webapp-chart/templates/ingress.yaml
    ```

    Here is an extract of the files contents:

    ```yml
    {{- if .Values.ingress.enabled -}}
    apiVersion: {{ include "common.capabilities.ingress.apiVersion" . }}
    kind: Ingress
    metadata:
      name: {{ include "aspnet-core.fullname" . }}
      labels: {{- include "common.labels.standard" . | nindent 4 }}
        ...
      annotations:
        ...
    spec:
      ...
    {{- end }}
    ```

    Notice how the Ingress manifest is dependent on the `{{- if .Values.ingress.enabled -}}` statement.

## Deploy the Helm Chart

You're now ready to deploy the web app. You'll run the `helm install` command, which runs the template engine to create the various manifest files and deploy the chart release.

1. Deploy the `drone-webapp` Helm chart:

    ```bash
    helm install drone-webapp ./drone-webapp-chart
    ```

    You can view the installation process by running the `kubectl get pods` command with the wait parameter. This parameter polls the result of the command and continuously updates the output.

    ```bash
    kubectl get pods -w
    ```

2. Once the web app is running, open the cluster's load balancer IP address in a browser to see the web app running. List the contents of the `create-aks-exports.txt` file to find the IP address of the load balancer:

    ```bash
    cat ~/clouddrive/mslearn-aks/create-aks-exports.txt
    ```

    Here is an example of the app running in a web browser:

    ![drone-web-app](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/7-web-app-browser.png)

## Upgrade the Helm Release

The development team updated the source code of the web app. To deploy a new version, you'll use the `helm upgrade` command to create a delta of the latest changes to the app.

1. Your first step is to list all Helm deployments by using the `helm list` command. This command allows you to see the current revision count for all Helm releases:

    ```bash
    helm list
    ```

    The command should return a result similar to the following:

    NAME | NAMESPACE | REVISION | UPDATED | STATUS | CHART | APP VERSION
    -----|-----------|----------|---------|--------|-------|------------
    drone-webapp | default | 1 | 2021-10-11 19:10:08.848253892 +0000 UTC | deployed | aspnet-core-1.3.18 | 0.0.1

2. Run the `helm history` command to view the history information about `drone-webapp`:

    ```bash
    helm history drone-webapp
    ```

    Output:

    REVISION | UPDATED | STATUS | CHART | APP VERSION | DESCRIPTION
    ---------|---------|--------|-------|-------------|------------
    1 | Mon Oct 11 19:10:08 2021 | deployed | aspnet-core-1.3.18 | 0.0.1 | Install complete

3. Open `Chart.yaml`:

    ```bash
    code ./drone-webapp-chart/Chart.yaml
    ```

    Update the `appVersion` number to version `0.0.2` and run the `helm upgrade` command:

    ```bash
    helm upgrade drone-webapp ./drone-webapp-chart
    ```

4. Run the `helm history` command to view the history information about `drone-webapp`:

    ```bash
    helm history drone-webapp
    ```

    The command should return a result similar to the following:

    REVISION | UPDATED | STATUS | CHART | APP VERSION | DESCRIPTION
    ---------|---------|--------|-------|-------------|------------
    1 | Mon Oct 11 19:10:08 2021 | superseded | aspnet-core-1.3.18 | 0.0.1 | Install complete
    2 | Mon Oct 11 19:18:06 2021 | deployed | aspnet-core-1.3.18 | 0.0.2 | Upgrade complete

## Roll Back the Helm Release

Since the upgrade of the release, a number of customers reported errors on the website. The team asks you to roll back the release to the previous stable version of the app. You'll use the `helm rollback` command to roll back the app.

1. Roll back the Helm release by using the `helm rollback` command, specifying the revision number of the rlease you're targeting:

    ```bash
    helm rollback drone-webapp 1
    ```

2. Once rollback is complete, view the Helm deployment history by using the `helm history` command:

    ```bash
    helm history drone-webapp
    ```

    The command should return a result similar to the following:

    REVISION | UPDATED | STATUS | CHART | APP VERSION | DESCRIPTION
    ---------|---------|--------|-------|-------------|------------
    1 | Mon Oct 11 19:10:08 2021 | superseded | aspnet-core-1.3.18 | 0.0.1 | Install complete
    2 | Mon Oct 11 19:18:06 2021 | superseded | aspnet-core-1.3.18 | 0.0.2 | Upgrade complete
    3 | Mon Oct 11 19:18:58 2021 | deployed | aspnet-core-1.3.18 | 0.0.1 | Rollback to 1

    Notice the last entry in the list shows:

    * An incremented revision number
    * The app version number of the rollback release
    * A description of the rollback release revision number

## Clean Up Resources

1. List the Resource Groups in the Azure CLI:

    ```bash
    az group list -o table
    ```

    Output:

    NAME | LOCATION | STATUS
    -----|----------|-------
    **learn-helm-deploy-aks-rg** | eastus | Succeeded

2. Delete the **learn-helm-deploy-aks-rg** resource group:

    ```bash
    az group delete -n learn-helm-deploy-aks-rg -y
    ```

3. Run the `kubectl config delete-context` command to remove the deleted cluster's context:

    ```bash
    kubectl config delete-context learn-helm-deploy-aks
    ```

    Output:

    ```
    deleted context learn-helm-deploy-aks from /home/user/.kube/config
    ```

## Revert .NET SDK Changes

This step isn't required when using thte sandbox. However, if you're running this exercise in your own subscription, review the following steps to revert any changes made to your Cloud Shell environment.

1. Run the following to restore your prior *.bashrc* profile startup file from a backup saved by the setup script:

    ```bash
    cp ~/.bashrc.bak.learn-helm-aks ~/.bashrc
    ```