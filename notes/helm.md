# Helm

Kubernetes allows you to manage the deployment lifecycle of cloud-native applications using a Kubernetes package manager. A Kubernetes package manager allows you to standardize, simplify, and implement reusable deployment strategies for your applications.

The deployment, versioning, and updating of any application usualy requires planning and management to ensure the correct versions of software libraries and configuration settings are deployed so that the deployed application functions as expected.

Suppose your team manages a deployed Kubernetes application with deployment, service, and ingress YAML-based files. The information in each file is hardcoded for each target environment and maintained by hand. Maintaining three files for each environment is cumbersome and increases complexity as the application grows.

![hardcoded-files](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-deploy-with-yaml-files.svg)

You can use Helm to simplify the application deployment processs and avoid hardcoded deployment variables and settings. Helm is a package manager for Kubernetes that combines all your application's resources and deployment information into a single deployment package.

![helm-chart](https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/media/2-what-is-helm.svg)

