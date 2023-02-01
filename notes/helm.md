# Helm

Kubernetes allows you to manage the deployment lifecycle of cloud-native applications using a Kubernetes package manager. A Kubernetes package manager allows you to standardize, simplify, and implement reusable deployment strategies for your applications.

The deployment, versioning, and updating of any application usualy requires planning and management to ensure the correct versions of software libraries and configuration settings are deployed so that the deployed application functions as expected.

Suppose your team manages a deployed Kubernetes application with deployment, service, and ingress YAML-based files. The information in each file is hardcoded for each target environment and maintained by hand. Maintaining three files for each environment is cumbersome and increases complexity as the application grows.

![hardcoded-files](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-deploy-with-yaml-files.svg)

You can use Helm to simplify the application deployment processs and avoid hardcoded deployment variables and settings. Helm is a package manager for Kubernetes that combines all your application's resources and deployment information into a single deployment package.

![helm-chart](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-what-is-helm.svg)

You can think of Helm in the same way as when installing applications using the Windows Package Manager on Windows, the Advanced Package Tool (apt) on Linux, or Homebrew on macOS. You specify the name of the application you want to install, update, or remove, and Helm takes care of the process.

Helm allows you to create templated, human-readable YAML script files to manage your application's deployment. These template files allwo you to specify all required dependencies, configuration mapping, and secrets used to manage the deploy of an application successfully.

Helm uses four components to manage applicatino deployments on a Kubernetes cluster.

* A Helm client
* Helm charts
* Helm releases
* Helm repositories

## Helm Client
[Back to Top](#helm)

A client installed binary responsible for creating and submitting manifest files required to deploy a Kubernetes application. The client is responsible for the interaction between the user and the Kubernetes cluster.

![helm-client](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-helm-components.svg)

The Helm client is available for all major operating systems and is installed on your client PC. In Azure, the Helm client is pre-installed in the Cloud Shell and supports all security, identity, and authorization features of Kubernetes.

## Helm Chart
[Back to Top](#helm)

A templated deployment package that describe a related set of Kubernetes resources. It contains all the information required to build and deploy the manifest files for an applicatino to run on a Kubernetes cluster.

A Helm chart consists of several files and folders to describe the chart. Some of the components are required, and some are optional. What you choose to include is based on the apps configuration requirements. Here is a list of files and folders with the required items in bold:

File / Folder | Description
--------------|------------
**`Chart.yaml`** | A YAML file containing the information about the chart.
**`values.yaml`** | The default configuration values for the chart.
**`templates/`** | A folder that contains the deployment templates for the chart.
`LICENSE` | A plain text file that contains the license for the chart.
`README.md` | A markdown file that contains instructions on how to use the chart.
`values.schema.json` | A schema file for applying structure on the values.yaml file.
`charts/` | A folder that contains all the subcharts to the main chart.
`crds` | Custom Resource Definitions.
`templates/Notes.txt` | A text file that contains template usage notes.

## Helm Release
[Back to Top](#helm)

The application or group of applications deployed using a chart. Each time you install a chart, a new instance of an application is created on the cluster. Each instance has a release name that allows you to interact with the specific application instance.

![helm-release](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-helm-components-release.svg)

Assume yuo've installed two Nginx instances onto your Kubernetes cluster using a chart. Later, you decide to upgrade the first Nginx instance, but not the second. Since the two releases are different, you can upgrade the first release without impacting the second.

## Helm Repository
[Back to Top](#helm)

A dedicated HTTP server that stores information on Helm charts. The server serves as a file that describes charts and where to download each chart.

![helm-repository](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-helm-components-repository.svg)

The Helm project hosts many public charts, and many repositories exist from which you can reuse charts. Helm repositories simplify the discoverability and reusability of Helm packages.

## Helm Benefits
[Back to Top](#helm)

Helm introduced a number of benefits that simplify application development and improves productivity in the development and deployment lifecycle of cloud-native applications. With Helm, you have application releases that are:

* Repeatable
* Reliable
* Manageable in multiple and complex environments
* Reusable across different development teams

A Helm chart standardizes the deployment of an application by using packaged template logic that is paramterized by set input values. The template driven package design provides an environment-agnostic approach to deploying and sharing cloud-native applications.

## Create and Install a Helm Chart
[Back to Top](#helm)

Helm makes it simple to deploy applications to any Kubernetes clusters by using Helm charts. You use Helm to template your application's deployment information as a Helm chart, which you then use to deploy your application.

Assume your development team already deploys your company's drone tracking website to your Azure Kubernetes Service cluster. The team creates three files to deploy the website.

* A deployment manifest that describes how to install and run the application on the cluster.
* A service manifest that describes how to expose the website on the cluster.
* An ingress manifest that describes how the traffic from outside the cluster is routed to the web app.

The team deploys these files to each of the three environments as part of the software development life cycle. Each of the three files is updated with variables and values that are specific to the environment. Since each file is hardcoded, the maintenance of these files is error-prone.

## How Does Helm Process a Chart?

The Helm client implements a Go language-based template engine that parses all available files in a chart's folders. The template engine creates Kubernetes manifest files by combining the templates in the chart's `templates/` folder with the values from the `Chart.yaml` and `values.yaml` files.

![helm-chart-process](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/4-helm-chart-process.svg)

Once the manifest files are available, the client can install, upgrade, and delete the application defined in the generated manifest files.

## How to Define a `Chart.yaml` File

The `Chart.yaml` is one of the required files in a Helm chart definition and provides information about the chart. The contents of the file constists of three required and various optional fields.

The three required fields are:

* The `apiVersion`. This value is the chart API version to use. You set the version to `v2` for Charts that use Helm 3.
* The `name` of the chart.
* The `version` of the chart. The version number uses semantic versioning 2.0.0 and follows the `MAJOR.MINOR.PATCH` version number notation.

Here is an example of a basic `Chart.yaml` file:

```yml
apiVersion: v2
name: webapp
description: A Helm chart for Kubernetes

# A chart can be either an 'application' or a 'library' chart.
#
# Application charts are a collection of templates that can be packaged into versioned archives
# to be deployed.
#
# Library charts provide useful utilities or functions for the chart developer. They're included as
# a dependency of application charts to inject those utilities and functions into the rendering
# pipeline. Library charts do not define any templates and therefore, cannot be deployed.
type: application

# This is the chart version. This version number should be incremented each time you make changes
# to the chart and its templates, including the app version.
version: 0.1.0

# This is the version number of the application being deployed. This version number should be
# incremented each time you make changes to the application.
appVersion: 1.0.0
```

Notice the inclusion of the `type` field above. You can create charts to install either applications or libraries. The default chart type is `application` and can be set to `library` to specify the chart will install a library.

Many optional fields are available to tailor the chart deployment process. For example, you can use the `dependencies` filed to specify additional requirements for the chart. For example, a web app that depends on a database.

## How to Define a Chart Template

A Helm Chart template is a file that describes different deployment type manifest files. Chart templates are written in the Go template language and provide additional template functions to automate the creation of Kubernetes object manifest files.

Template files are stored in the `templates/` folder of a chart and processed by thet template engine to create the final object manifest.

For example, the development team uses the following deployment manifest to deploy the drone tracking website:

```yml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: webapp
  labels:
    app: dronetracker
    service: webapp
spec:
  replicas: 1
  selector:
    matchLabels:
      service: webapp
  template:
    metadata:
      labels:
        app: dronetracker
        service: webapp
    spec:
      containers:
        - name: webapp
          image: my-acr-registry.azurecr.io/webapp:linux-v1
          imagePullPolicy: Always
          resources:
            requests:
              cpu: 100m
              memory: 128Mi
            limits:
              cpu: 250m
              memory: 256Mi
          ports:
            - name: http
              containerPort: 80
              protocol: TCP
```

Notice how the location of the container image is hardcoded. Helm charts allow you to define manifest templates with value placeholders to avoid hard coding values.

For example, the deployment teams wants to allow for install time configuration values. The container registry, docker releast tag, and Kubernetes pull policy should be configurable in the template. To allow for this configuration, you can modify the existing manifest file iwth the following example template syntax:

```yml
apiVersion: apps/v1
kind: Deployment
metadata:
    ...
  spec:
    ...
    template:
      ...
      spec:
        containers:
          - name: webapp
            image: {{ .Values.registry }}/webapp:{{ .Values.dockerTag }}
            imagePullPolicy: {{ .Values.pullPolicy }}
            resources:
            ...
            ports:
              ...
```

Notice the use of the `{{ .Values.<property> }}` syntax. The syntax allows you to create placeholders for each custom value.

The process of creating Helm charts by hand is tedious. An easy way to create a Helm chart is to use the `helm create` command to create a new Helm chart. You then customize the autogenerated files to match your application's requirements.

## How to Define a `values.yaml` File

You use chart values to customize the configuration of a Helm chart. Chart values can either be predefined or supplied by the user at the time of deploying the chart.

A **predefined** value is a case-sensitive value that is predefined in the context of a Helm Chart and can't be changed by a user. For example, you can use `Release.Name` to reference the name of the release, or `Release.IsInstall` to check if the current operation is an installation.

You may also use predefined values to extract data from the contents of the `Chart.yaml`. For example, if you want to check the chart's version, you'd reference `Chart.Version`. Keep in mind that you can only reference well-known fields. You can think of predefined values as constants to use in the templates you create.

The syntax to include value names in a template file is done by enclosing the value name in double curly braces, for example, `{{.Release.Name}}`. Notice the use of a period in front of the value name. When you use a period in this way, the period functions as a lookup operator and indicates the variable's current scope.

For example, the following YAML snippet contains a dictionary defined in a values file:

```yml
object:
  key: value
```

To access the value in a template, you can use the following syntax:

```yml
{{ .Values.object.key }}
```

A **Supplied** value allows you to process arbitrary values in the chart template. The `values.yaml` file defines these values.

IN the xamples, the development team allows for three configurable values. A container registry name (`registry`), a docker release tag (`dockerTag`), and a Kubernetes pull policy (`pullPolicy`).

Here's an example of the `values.yaml` file:

```yml
apiVersion: v2
name: webapp
description: A Helm chart for Kubernetes
...
registry: "my-acr-registry.azurecr.io"
dockerTag: "linux-v1"
pullPolicy: "Always"
```

## How to Use a Helm Repository

A Helm repository is a dedicated HTTP server that stores information on Helm charts. You configure Helm repositories with the Helm client for it to install charts from a repository using the `helm repo add` command.

For example, to add the ASP.NET Core chart available from the marketplace Helm repository, you can run the following:

```bash
helm repo add azure-marketplace https://marketplace.azurecr.io/helm/v1/repo
```

Information about charts available on a repository is cached on the client host. You'll need to periodically update the cache manually to fetch the repository's latest information by running the `helm repo update` command.

The `helm search repo` command allows you to search for charts on all locally added Helm repositories. You can run the `helm search repo` command by itself to return a list of all known Helm charts for each added repository. The result lists the chart's name, version, and app version deployed by the chart, as shown here:

NAME | CHART VERSION | APP VERSION | DESCRIPTION
-----|---------------|-------------|------------
azure-marketplace/airflow | 11.0.8 | 2.1.4 | Apache Airflow is a platform to programmaticall...
azure-marketplace/apache | 8.8.3 | 2.4.50 | Chart for Apache HTTP Server
azure-marketplace/aspnet-core | 1.3.18 | 3.1.19 | ASP.NET Core is an open-source framework create...
azure-marketplace/bitnami-common | 0.0.7 | 0.0.7 | Chart with custom tempaltes used in Bitnami cha...
azure-marketplace/cassandra | 8.0.5 | 4.0.1 | Apache Cassandra is a free and open-source dist...

You can search for a specific chart by adding a search term to the `helm search repo` command. For example, if you're searching for an ASP.NET based chart, you can use the following:

```bash
helm search repo aspnet
```

The local client has two repositories registered and returns a result from each:

NAME | CHART VERSION | APP VERSION | DESCRIPTION
-----|---------------|-------------|------------
azure-marketplace/aspnet-core | 1.3.18 | 3.1.19 | ASP.NET Core is an open-source framework create...
bitnami/aspnet-core | 1.3.18 | 3.1.19 | ASP.NET Core is an open-source framework create...

## How to Test a Helm Chart

Helm provides an option for you to generate the manifest files that the template engine creates from the chart. This feature allows you to test the chart before a release by combining two additional parameters. These parameters are `--dry-run` and `--debug`.

The `--dry-run` parameter makes sure that the installation is simulated, and the `--debug` parameter enables verbose output. Here's an example:

```bash
helm install --debug --dry-run my-drone-webapp ./drone-webapp
```

The command lists information about the values used and all generated files.

## How to Install a Helm Chart

You use the `helm install` command to install a chart. A Helm chart can be installed from any of the following locations:

* Chart folder
* A packaged `.tgz` tar archive chart
* A Helm repository

However, the required parameters differ depending on the location of the chart. In all cases, the install command requires the name of the chart you want to install and a name for the release the installation will create.

A local chart can be installed using an unpacked chart folder of files or a packed `.tgz` archive. To install a chart, thet helm command references the local file system for the chart's location. Here ris an example of the install command that will deploy a release of an unpacked chart with the name `drone-webapp`.

```bash
helm install my-drone-webapp ./drone-webapp
```

In the above example, the `my-drone-webapp` parameter is the name of the release and the `./my-drone-webapp` parameter is the name of the unpacked chart package.

A packed chart is installed by referencing the packed chart filename. The following example shows the syntax for the same application now packed as a tar archive.

```bash
helm install my-drone-webapp ./drone-webapp.tgz
```

When installing a chart from a Helm repository, you use a chart reference as the chart's name. The chart reference includes two parameters: the repository name and the name of the chart.

The following install command installs a default configured ASP.NET Core application from the marketplace Helm repository.

```bash
helm install my-release azure-marketplace/aspnet-core
```

In the example, the `azure-marketplace/aspnet-core` parameter contains the rerference to the repo `azure-marketplace`, and the  chart `aspnet-core`.

## Manage a Helm Release

Helm charts make it easy to install, upgrade, roll back, and delete an application on a Kubernetes cluster. Helm also provides functionality to retrieve the status of a Helm release. This feature allows you to track changes to running releases and manage future deployments. You track aspects such as last deployment time or list the resources used in the current release.

Application code seldom remains static and change because of new features or occasional bug fixes. Sometimes, new features also add dependencies to an application. These changes will require you to update your charts and use Helm's chart management features to upgrade or roll back an existing release.

Here, you'll learn how to add chart dependencies, upgrade a Helm release, and roll back a Helm release.

To modify your first Helm chart, we need to cover some of the Helm template language concepts.

## Template Functions and Pipelines

> Docs: [Template Functions and Pipelines](https://helm.sh/docs/chart_template_guide/functions_and_pipelines/)

The Helm template language defines **functions** that you use to transform values form the `values.yaml` file. The syntax for a function follows the `{{ functionName arg1 arg2 }}` structure. Let's look at the `quote` function as an example to see this syntax in use.

The `quote` function wraps a value in quotation marks to indicate the use of a `string`. Assume you define the following `values.yaml` file:

```yml
apiVersion: 2
name: webapp
description: A Helm chart for Kubernetes
ingress:
  enabled: true
```

You decide you want to use the `ingress.enabled` value as a boolean when determining if an ingress manifest should be generated. To use the `enabled` value as a boolean, you reference the value using `{{ .Values.ingress.enabled }}`.

Later, you decide to display the field as a string in the `templates/Notes.txt` file. Since YAML type coercion rules can lead to hard to find bugs in templates, you decided to follow guidance and be explicity when including strings in your templates. For example, `enabled: false` doesn't equal `enabled: "false"`.

To display the value as a string, you use `{{ quote .Values.ingress.enabled }}` to reference the boolean value as a string.

You use **pipelines** when more than one function needs to act on a value. A pipeline allows you to *send* a value, or the result of a function, to another function. For example, you can rewrite the above `quote` function as `{{ .Values.ingress.enabled | quote }}`. Notice how the `|` indicates that the value is *sent* to the `quote` function.

Here is another example, assume you want to convert a value to uppercase and wrap it in quotes. You can write the statement as `{{ .Values.ingress.enabled | upper | quote }}`. Notice how the value is processed by the `upper` function and then the `quote` function.

The template language includes over 60 functions that allow you to expose, look up, and transform values and objects in templates.

> Docs: [Template Function List](https://helm.sh/docs/chart_template_guide/function_list/)

## Conditional Flow Control in a Template

Conditional flow control allows you to decide the structure or data included in the generated manifest file. 

The `if / else` block is such a control flow structure and conforms to the following layout:

```yml
{{ if | pipeline | }}
    # Do something
{{ else if | other pipeline | }}
    # do something
{{ else }}
    # default case
{{ end }}
```

Assume you decide that the ingress manifest file for a chart is only created in specific cases:

```yml
{{ if .Values.ingress.enabled }}
apiVersion: extensions/v1
kind: Ingress
metadata:
  name: ..
  labels:
    ...
  annotations:
    ...
spec:
  rules:
    ...
{{ end }}
```

Recall that you can use placeholders to populate metadata in thet template. Template files are parsed and evaluated sequentially by the template language from top to bottom. In the above example, the template engine only generates the manifest file's contents if the `.Values.ingress.enabled` value is `true`.

When the template engine processes the statement, it removes the content declared inside the `{{ }}` syntax, and leaves the remaining whitespace. This syntax causes the template engine to include a newline for the `if` statement line. If you leave the above file's contents, you'll notice empty lines in your YAML, and the ingress manifest file is generated.

YAML gives meaning to whitespace. That's why tab, space, and newline characters are considered important. To fix the problem of unwanted whitespace, you can rewrite the file as follows:

```yml
{{- if .Values.ingress.enabled -}}
apiVersion: extnesions/v1
kind: Ingress
metadata:
  name: ..
  labels:
    ...
  annotations:
    ...
spec:
  rules:
    ...
{{- end }}
```

Notice the use of the `-` character as part of the start `{{-` and the end `-}}` sequence of the statement. The `-` character instructs the parser to remove whitespace characters. `{{-` removes whitespace at the start of a line and `-}}` at the end of a line, including the newline character.

## Template Collection Iteration

YAML allows you to define collections of items and use individual items as values in your templates. Accessing items in a collection is possible using an indexer. However, the Helm template language supports the iteration of a collection of values using the `range` operator.

Assume you define a list of values in your `values.yaml` file to indicate additional ingress hosts. Here is an example of the values:

```yml
ingress:
  enabled: true
  extraHosts:
    - name: host1.local
      path: /
    - name: host2.local
      path: /
    - name: host3.local
      path: /
```

You use the range operator to allow the template engine to iterate through `.Values.ingress.extraHosts`:

```yml
{{- if .Values.ingress.enabled -}}
apiVersion: extensions/v1
kind: Ingress
metadata:
  ...
spec:
  rules:
    ...
    {{- range .Values.ingress.extraHosts }}
    - host: {{ .name }}
      http:
        paths:
          - path: {{ .path }}
            ...
    {{- end }}
  ...
{{- end }}
```

## Control Value Scope in Templates

When you have values defined several layers deep, your syntax may become lengthy and cumbersome when including these values in a template. The `with` action allows you to limit the scope of variables in a template.

The `.` used in a Helm template references the current scope. For example, `.Values` instructs the template engine to find the Values object in the current scope. Assume you're using the `values.yaml` file shown to create a configuration map manifest file:

```yml
ingress:
  enabled: true
  extraHosts:
    - name: host1.local
      path: /
    - name: host2.local
      path: /
    - name: host3.local
      path: /
```

Instead of accessing each item's path value using `{{ .Values.ingress.extraHosts.path }}`, you can use the `with` action.

```yml
apiVersion: v1
kind: ConfigMap
metadata:
  name: {{ .Release.Name }}-configmap
data:
  {{- with .Values.ingress.extraHosts }}
  hostname: {{ .name }}
  path: {{ .path }}
  {{ end }}
```

`{{- with .Values.ingress.extraHosts }}` limits the scope of values to the `Values.ingress.extraHosts` array.

The `with` action restricts scope. You can't access other objects from the parent scope. Assume you also want to access the `{{ .Release.Name }}` of the chart in the `with` code block. To access parent objects, you need to indicate the root scope by using the `$` character or rewrite your code.

Including parent objects using the `$` character:

```yml
apiVersion: v1
kind: ConfigMap
metadata:
  name: {{ .Release.Name }}-configmap
data:
  {{- with .Values.ingress.extraHosts }}
  hostname: {{ .name }}
  path: {{ .path }}
  release: {{ $.Release.Name}}
  {{ end }}
```

## Define Chart Dependencies

A chart allows for the declaration of dependencies to support the main application and forms part of the installed release. For example, the development team decides to add a database to the web application. The web app is now dependent on the database's availability to function properly.

![helm-deploy-subchart-dependencies](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/6-helm-deploy-subchart-dependencies.svg)

You can either create a subchart using the `helm create` command, specifying the new chart's location in the `/charts` folder, or use the `helm dependency` command. Recall from earlier that the `/charts` folder may contain subcharts deployed ass part of the main chart's release.

The `helm dependency` command allows you to manage dependencies included from a Helm repository. The command uses metadata defined in the `dependencies` section of your chart's values file. You specify the name, version number, and the repository from where to install the sub chart. Here is an extract of a `values.yaml` file that has a MongoDB chart listed as a dependency:

```yml
apiVersion: v2
name: my-app
description: A Helm chart for Kubernetes
...
dependencies:
  - name: mongodb
    version: 10.27.2
    repository: https://marketplace.azurecr.io/helm/v1/repo
```

Once the dependency metadata is defined, you run the `helm dependency build` command to fetch the tar packaged chart. The chart build command downloads the chart into the `charts/` folder.

Here is an example of what the command may look like when run:

```bash
helm dependency build ./app-chart
```

Sub-charts are managed separately from the main chart and may need updates as new releases become available. The command to update subcharts is `helm dependency update`. This command will fetch new versions of the sub-chart while deleting outdated packages.

Here is an example of what the commmand many look like when run:

```bash
helm dependency update ./app-chart
```

Keep in mind, a chart dependency isn't limited to other applications. You may decide to reuse template logic across your charts and create a dependency specific to manage this logic as a chart dependency.

## How to Upgrade a Helm Release

Helm allows upgrading existing releases as a delta of all the changes that apply to the chart and its dependencies.

![helm-delta-release](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/6-helm-delta-release.svg)

For example, assume your team deployed the drone tracking application and the database supporting the website. The team makes code changes that impact only the website's functionality. To prepare for the deployment of the new application, the team completes the following tasks:

* First, the development team updates the web app's container image and tags the image as `webapp: linux-v2` when pushing the image to the container registry.

* Next, the team updates the `dockerTag` value to `linux-v2` and the chart `version` value to `0.2.0` in the chart's values file.

Example `values.yaml` file after changes:

```yml
apiVersion: v2
name: my-app
description: A Helm chart for Kubernetes

type: application

version: 0.2.0
appVersion: 1.0.0

registry: "my-acr-registry.azurecr.io"
dockerTag: "linux-v2"
pullPolicy: "Always"
```

Instead of uninstalling the current release, you'll use the `helm upgrade` command to upgrade the existing Helm release:

```bash
helm upgrade my-app ./app-chart
```

Helm generates a delta of the changes made to the Helm chart when you upgrade a release. As such, a Helm upgrade will only upgrade the components identified in the delta. In the example, only the website is redeployed.

Once the upgrade completes, you can review the deployment history of the release using the `helm history` command. This command uses the release name as an additional parameter to return the release history:

```bash
helm history my-app
```

The history command returns several fields that describe the release:

REVISION | UPDATED | STATUS | CHART | APP VERSION | DESCRIPTION
---------|---------|--------|-------|-------------|------------
1 | Mon Oct 11 17:25:33 2021 | deployed | aspnet-core-1.3.18 | 3.1.19 | Install complete

Notice the `revision` field in the result. Helm tracks release information of all releases done for a Helm chart. When you install a new version of a Helm chart, the revision count increases by one, and the new release information is matched to that revision.

Here's an example of the same history command after a new version the web app is installed:

REVISION | UPDATED | STATUS | CHART | APP VERSION | DESCRIPTION
---------|---------|--------|-------|-------------|------------
1 | Mon Oct 11 17:25:33 2021 | superseded | aspnet-core-1.3.18 | 3.1.19 | Install complete
2 | Mon Oct 11 17:35:13 2021 | deployed | aspnet-core-1.3.18 | 3.1.19 | Upgrade complete

## How to Roll Back a Helm Release

Helm allows the rollback of an existing Helm release to a previously installed release. Recall from earlier, Helm tracks release information of all releases of a Helm chart.

You use the `helm rollback` command to roll back to a specific Helm release revision. This command uses two parameters. The first parameter identifies the name of the release, and the second command identifies the release revision number. Here is an example of the command:

```bash
helm rollback my-app 2
```

The `helm rollback` command will roll back the release version of the app to the specified revision and update the app release history. A follow-on run of the `helm history` command will show the latest active revision number as the latest release entry.

For example, the development team discovers a bug in the drone tracking web application and needs to roll back to a previous release. Instead of uninstalling the current release and install a previous one, you can roll back to the working release:

```bash
helm rollback my-app 1
```

Once the rollback completes, you can review the deployment history using the `helm history` command:

REVISION | UPDATED | STATUS | CHART | APP VERSION | DESCRIPTION
---------|---------|--------|-------|-------------|------------
1 | Mon Oct 11 17:25:33 2021 | superseded | aspnet-core-1.3.18 | 3.1.19 | Install complete
2 | Mon Oct 11 17:38:13 2021 | superseded | aspnet-core-1.3.18 | 3.1.19 | Upgrade complete
3 | Mon Oct 11 17:35:13 2021 | deployed | aspnet-core-1.3.18 | 3.1.19 | Rolled back to 1