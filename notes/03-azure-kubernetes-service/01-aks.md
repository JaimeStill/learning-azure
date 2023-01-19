# Azure Kubernetes Service

* [Creating an AKS Cluster](#creating-an-aks-cluster)
* [How Workloads are Developed and Deployed to AKS](#how-workloads-are-developed-and-deployed-to-aks)
* [Bridge to Kubernetes](#bridge-to-kubernetes)
* [Deployment Center](#deployment-center)
* [Azure Service Integration](#azure-service-integration)
* [When to Use AKS](#when-to-use-aks)

Azure Kubernetes Service (AKS) manages your hosted Kubernetes environment and makes it simple to deploy and manage containerized applications in Azure. Your AKS environment is enabled with features such as automated updates, self-healing, and easy scaling. The Kubernetes cluster master is managed by Azure and is free. You managae the agent nodes in the cluster and only pay for the VMs on which your nodes run.

You can either create your cluster in the Azure portal or use the Azure CLI. When you create the cluster, you can use Resource Manager templates to automate cluster creation. With these templates, you specify features such as advanced networking, Azure Active Directory (AD) integration, and monitoring. This information is then used to automate the cluster deployment on your behalf.

With AKS, we get the benefits of open-source Kubernetes without the complexity or operational overhead compared to running our own custom Kubernetes cluster.

## Creating an AKS Cluster
[Back to Top](#azure-kubernetes-service)

At its core, an AKS cluster is a cloud hosted Kubernetes cluster. Unlike a common Kubernetes installation, AKS streamlines the installation process and takes care of most of the underlying cluster management tasks.

You have two options when you create an AKS cluster. You either use the Azure portal or Azure CLI. Both options require you to configure basic information about the cluster:

* The Kubernetes cluster name
* The version of Kubernetes to install
* A DNS prefix to make the master node publicly accessible
* The initial node pool size

The initial node pool size defaults to two nodes, however it's recommended that at least three nodes are used for a production environment.

Unless specified, the Azure service creation workflow creates a Kubernetes cluster usig ndefault configuration for scaling, authentication, networking, and monitoring. Creating an AKS cluster typically takes a few minutes. Once complete, you can change any of the default AKS cluster properties. Access and managmenet of your cluster is done through the Azure portal or from the command line.

## How Workloads are Developed and Deployed to AKS
[Back to Top](#azure-kubernetes-service)

![development-accelerate](https://learn.microsoft.com/en-us/training/modules/intro-to-azure-kubernetes-service/media/3-development-accelerate.png)

AKS supports the Docker image format. That means that you can use any development environment to create a workload, package the workload as a container, and deploy the container as a Kubernetes pod.

Here you use the standard Kubernetes command-line tools or the Azure CLI to manage your deployments. The support for the standard Kubernetes tools ensures that you don't need to change your current workflow to support an existing Kubernetes migration to AKS.

AKS also supports all the popular development and management tools such as Helm, Draft, Kubernetes extension fro Visual Studio Code, and Visual Studio Kubernetes Tools.

## Bridge to Kubernetes
[Back to Top](#azure-kubernetes-service)

Allows you to run and debug code on your development computer, while still connected to your Kubernetes cluster with the rest of your application or services.

Using Bridge to Kubernetes lets you:

* Avoid having to build and deploy code to your cluster by instead creating a direct connection from your development computer to your cluster, allowing you to quickly test and develop your service in the contxt of the full application without creating any Docker or Kubernetes configuration.
* Redirect traffic between your connected Kubernetes cluster and your development computer, which allows code on your development computer and services running in your Kubernetes cluster to communicate as if they are in the same Kubernetes cluster.
* Provide a way to replicate environment variabless and mounted volumes available to pods in your Kubernetes cluster in your development computer, which allows you to quickly work on your code without having to replicate those dependencies manually.

## Deployment Center
[Back to Top](#azure-kubernetes-service)

Simplifies setting up a DevOps pipeline for your application. You can use this configured DevOps pipeline to setup a continuous integration (CI) and continuous delivery (CD) pipeline to your AKS Kubernetes cluter.

With Azure DevOps Projects you can:

* Automatically create Azure resources, such as an AKS cluster
* Create an Azure Application Insights resource for monitoring an AKS cluster
* Enable Azure Monitor for containers to monitor performance for the container workloads on an AKS cluster

You can add richer DevOps capabilities by extending the default configured DevOps pipeline. For example, you can add approvals before deploying, provision additional Azure resources run scripts or upgrade workloads.

## Azure Service Integration
[Back to Top](#azure-kubernetes-service)

AKS allows us to integrate any Azure service offering and use it as part of an AKS cluster solution.

Kubernetes doesn't provide middleware and storage systems. Suppose you need to add a processing queue to the fleet management data processing service. You can easily integrate Storage queues using Azure Storage to extend the capacity of the data processing service.

## When to Use AKS

Requirement | Consideration
------------|--------------
**Identity and security management** | Do you already use existing Azure resources and make use of Azure AD? YOu can configure an AKS cluster to integrate with Azure AD and reuse existing identities and group membership.
**Integrated logging and monitoring** | AKS includes Azure Monitor for containers to provide performance visiblity to the cluster. With custom Kubernetes installation, you normally decided on a monitoring solution that requires installation and configuration.
**Auto Cluster node and pod scaling** | Deciding when to scale up or down in large containerization environment is tricky. AKS supports two auto cluster scaling options. You can use either the horizontal pod autoscaler or the cluster autoscaler to scale the cluster. The horizontal pod autoscaler watches the resource demand of pods and will increase pods to match demand. THe cluster autoscaler component watches for pods that can't be scheduled because of node constraints. It will automatically scale cluster nodes to deploy scheduled pods.
**Cluster node upgrades** | Do you want to reduce the number of cluster management tasks? AKS manages Kubernetes software upgrades and the process of cordoning off nodes and draining them to minimize disruption to running applications. Once done, these nodes are upgraded one by one.
**GPU enabled nodes** | Do you have compute-intensive or graphic-intensive workloads? AKS supports GPU enabled node pools.
**Storage volume support** | Is your application stateful, and does it require persisted storage? AKS supports both static and dynamic storage volumes. Pods can attach and reattach to these storage volumes as they're created or rescheduled on different nodes.
**Virtual network support** | Do you need pod to pod network communication or access to on-premise networks from you AKS cluster? An AKS cluster can be deployed into an existing virtual network with ease.
**Ingress with HTTP application routing support** | Do you need to make deployed applications publicly available? The HTTP application routing add-on makes it easy to access AKS cluster deployed applications.
**Docker image support** | Do you already use Docker images for your containers? AKS by default supports the Docker file image format.
**Private container registry** | Do you need a private container registry? AKS integrates with Azure Container Registry (ACR). You aren't limited to ACR though, you can use other container repositories, public or private.