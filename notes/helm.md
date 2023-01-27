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

The application or group of applications deployed using a chart. Each time you install a chart, a new instance of an application is created on the cluster. Each instance has a release name that allows you to interact with the specific application instance.

![helm-release](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-helm-components-release.svg)

Assume yuo've installed two Nginx instances onto your Kubernetes cluster using a chart. Later, you decide to upgrade the first Nginx instance, but not the second. Since the two releases are different, you can upgrade the first release without impacting the second.

## Helm Repository

A dedicated HTTP server that stores information on Helm charts. The server serves as a file that describes charts and where to download each chart.

![helm-repository](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-helm-components-repository.svg)

The Helm project hosts many public charts, and many repositories exist from which you can reuse charts. Helm repositories simplify the discoverability and reusability of Helm packages.

## Helm Benefits

Helm introduced a number of benefits that simplify application development and improves productivity in the development and deployment lifecycle of cloud-native applications. With Helm, you have application releases that are:

* Repeatable
* Reliable
* Manageable in multiple and complex environments
* Reusable across different development teams

A Helm chart standardizes the deployment of an application by using packaged template logic that is paramterized by set input values. The template driven package design provides an environment-agnostic approach to deploying and sharing cloud-native applications.