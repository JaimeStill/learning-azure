# Deploy a Containerized Application on Azure Kubernetes Service

* [Kubernetes Clusters](#kubernetes-clusters)
* [Cluster Nodes](#cluster-nodes)
* [Cluster Architectures](#cluster-architectures)
    * [Single Control Plane and Multiple Nodes](#single-control-plane-and-multiple-nodes)
    * [Single Control Plane and a Single Node](#single-control-plane-and-a-single-node)
* [AKS Cluster Configuration](#aks-cluster-configuration)
    * [Node Pools](#node-pools)
    * [Node Count](#node-count)
    * [Automatic Routing](#automatic-routing)
        * [Ingress Controllers](#ingress-controllers)
* [Create an AKS Cluster](#create-an-aks-cluster)
    * [Link with `kubectl`](#link-with-kubectl)
* [Deployment Overview](#deployment-overview)
    * [Container Registry](#container-registry)
    * [Kubernetes Pod](#kubernetes-pod)
    * [Kubernetes Deployment](#kubernetes-deployment)
        * [Manifest Files](#manifest-files)
        * [Kubernetes Label](#kubernetes-label)
        * [Manifest File Structure](#manifest-file-structure)
        * [Group Objects in a Deployment](#group-objects-in-a-deployment)
        * [Apply a Deployment File](#apply-a-deployment-file)
* [Deploy an App to AKS](#deploy-an-app-to-aks)
    * [Create a Deployment Manifest](#create-a-deployment-manifest)
    * [Apply the Manifest](#apply-the-manifest)

Before you can deploy an application, you need to create an AKS cluster. The following review covers concepts that allow you to deploy a new AKS cluster successfully.

## Kubernetes Clusters
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Kubernetes is based on clusters. Instead of haivng a single virtual machine (VM), it uses several machines working as one. These VMs are called nodes. Kubernetes is a cluster-based orchestrator. It provides your application with several benefits, such as availability, monitoring, scaling, and rolling updates.

## Cluster Nodes
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

A cluster is node-based. There are two types of nodes in a Kubernetes cluster that provide specific functionality.

* **Control plane nodes:** These nodes host the cluster's control plane aspects and are reserved for services that control the cluster. They're responsible for providing the API you and all the other nodes use to communicate. No workloads are deployed or scheduled on these nodes.

* **Nodes:** These nodes are responsible for executing custom workloads and applications, such as components from a cloud-based video rendering service.

## Cluster Architectures
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

A cluster architecture allows you to conceptualize the number of control planes and nodes you'll deploy in your Kubernetes cluster.

For example, the number of nodes in a cluster should always be more than two. When a node becomes unavailable, the Kubernetes scheduler will try to reschedule all the workloads running on this node onto the remaining nodes in the cluster.

There are two popular cluster architectures for Kubernetes-based deployments:

* Single control plane and multiple nodes
* Single control plane and a single node

### Single Control Plane and Multiple Nodes
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

![single-cp-multiple-nodes](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/2-1-diagram.png)

This architecture is the most common architectural pattern, and is the easiest to deploy, but it doesn't provide high availability to your cluster's core management services.

If the control plane node becomes unavailable for some reason, no other interaction can happen with the cluster. This is the case even by you as an operator, or by any workloads that user Kubernetes' APIs to communicate until, at least, the API server is back online.

Despite being less available than others, this architecture should be enough for most situations. It's less likely that the core management services become unavailable compared to a node going offline. The control plane nodes are subject to fewer modifications than nodes and are more resilient.

If you're dealing with a produciton scenario, this architecture might not be the best solution.

### Single Control Plane and a Single Node
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

![single-cp-single-node](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/2-2-single-diagram.png)

This architecture is a variant of the previous architcture and is used in development environments. This architecture provides only one node that hosts both the control plane and a worker node. It's useful when testing or experimenting with different Kubernetes concepts. The single control plane and single node architecture limits cluster scaling and makes this architecture unsuitable for production and staging use.

## AKS Cluster Configuration
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

When you create a new AKS cluster, you have different items of information that you need to configure. Each item affects the final configuration of your cluster.

These items include:
* Node pools
* Node count
* Automatic routing

### Node Pools
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

![node-pool-diagram](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/2-3-node-pool-diagram.png)

You create *node pools* to group nodes in your AKS cluster. When you create a node pool, you specify the VM size and OS type (Linux or Windows) for each node in the node pool based on application requirement. To host user application pods, node pool **Mode** should be **User** otherwise **System**.

By default, an AKS cluster will have a Linux node pool (**System Mode**) whether it's created through Azure portal or CLI. However, you'll always have an option to add Windows node pools along with default Linux node pools durin the creation wizard in the portal, via CLI, or in ARM templates.

Node pools use Virtual Machine Scale Sets as the underlying infrastructure to allow the cluster to scale the number of nodes in a node pool. New nodes created in the node pool will always be the same size as you specified when you created the node pool.

### Node Count
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

The number of nodes your cluster will have in a node pool. Nodes are Azure VMs. You can change their size and count to match your usage pattern.

You can change the node count later in the cluster's configuration panel. It's also a best practice to keep this number as low as possible to avoid unnecessary costs and unused compute power.

### Automatic Routing
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

A Kubernetes cluster blocks all external communications by default. For example, assume you deploy a website that's accessible to all clients. You need to manually create an *ingress* with an exception that allows incoming client connections to that particular service. This configuration requires network-related changes that forward requests from the client to an internal IP on the cluster, and finally to your application. Depending on your specific requirements, this process can be complicated.

AKS allows you to overcome the complexity by enabling what's called HTTP application routing. This add-on makes it easy to access applications on the cluster through an automatically deployed ingress controller.

#### Ingress Controllers
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

![http-app-routing-diagrm](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/2-4-http-application-routing-diagram.png)

Ingress controllers provide the capability to deploy and expose your applications to the world without the need to configure network-related services.

Ingress controllers create a reverse-proxy server that automatically allows for all the requests to be servved from a single DNS output. You don't have to create a DNS record every time a new service is deployed. The ingress controller will take care of it. When a new ingress is deployed to the cluster, the ingress controller creates a new record on an Azure managed DNS zone and links it to an existing load balancer. This functionality allows for easy to access to the resource through the internet without the need for additional configuration.

Despite the advantages, HTTP application routing is better suited to more basic clusters. It doesn't provide the amount of customization needed for a more complex configuration. If you plan to deal with more complex clusters, there are better-suited options like the official Kubernetes ingress controller.

## Create an AKS Cluster
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

1. Sign in to Azure in WSL

    ```bash
    az login
    ```

2. Create variables for the configuration values you'll reuse throughout the exercises.

    ```bash
    export RESOURCE_GROUP=rg-contoso-video
    export CLUSTER_NAME=aks-contoso-video
    export LOCATION=eastus
    ```

3. Run the `az group create` command to create a resource group. You'll deploy all resources into this new resource group.

    ```bash
    az group create --name=$RESOURCE_GROUP --location=$LOCATION
    ```

4. Run the `az aks create` command to creae an AKS cluster.

    **Saved as [create-aks.sh](./assets/create-aks.sh)**

    ```bash
    az aks create \
        --resource-group $RESOURCE_GROUP \
        --name $CLUSTER_NAME \
        --node-count 2\
        --enable-addons https_application_routing \
        --generate-ssh-keys \
        --node-vm-size Standard_B2s \
        --network-plugin azure \
        --windows-admin-username localadmin
    ```

    The command creates a new AKS cluster named `aks-contoso-video` with the `rg-contoso-video` resource group. The cluster will have two nodes defined by the `--node-count` parameter. We're using only two nodes in this exercise for cost considerations. The `--node-vm-size` parameter configures the cluster nodes as *Standard_B2s*-sized VMs. The HTTP application routing add-on is enabled via the `--enable-addons` flag. These nodes will be part of **System** mode.

    The `--windows-admin-username` parameter is used to setup administrator credentials for Windows containers, and prompts the user to set a password at the command line. the password as to meet [Windows Server password requirements](https://learn.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/password-must-meet-complexity-requirements#reference).

5. Run the `az aks nodepool add` command to add another node pool that uses the Windows operating system.

    **Saved as [add-aks-nodepool.sh](./assets/add-aks-nodepool.sh)**

    ```bash
    az aks nodepool add \
        --resource-group $RESOURCE_GROUP \
        --cluster-name $CLUSTER_NAME \
        --name uspool \
        --node-count 2 \
        --node-vm-size Standard_B2s \
        --os-type Windows
    ```

    The command adds a new node pool (**User** mode) to an existing AKS cluster. This new node pool can be used to host applications and workloads, instead of using the **System** node pool, which was created using `az aks create` above.

    The `--os-type` parameter is used to specify the operating system of the node pool. If not specified, the command will use Linux as the operating system for the nodes.

### Link with `kubectl`
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

1. Link your Kubernetes cluster with `kubectl` by running the following command:

    ```bash
    az aks get-credentials --name $CLUSTER_NAME --resource-group $RESOURCE_GROUP

    # copy from Windows users directory to wsl user directory
    cp /mnt/c/Users/{user}/.kube/config ~/.kube/config
    ```

    This command will add an entry to your `~/.kube/config` file (see [kube.config](./assets/kube.config) for a sanitized example), which holds all the information to access your clusters. Kubectl enables you to manage multiple clusters from a single command-line interface.

2. Run the `kubectl get nodes` command to check that you can connect to your cluster, and confirm its configuration.

    ```bash
    kubectl get nodes
    ```

    Output:

    NAME | STATUS | ROLES | AGE | VERSION
    -----|--------|-------|-----|--------
    aks-nodepool1-16797674-vmss000000 | Ready | agent | 39m | v1.24.6
    aks-nodepool1-16797674-vmss000001 | Ready | agent | 39m | v1.24.6
    aksuspool000000 | Ready | agent | 29m | v1.24.6
    aksuspool000001 | Ready | agent | 30m | v1.24.6

## Deployment Overview
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

With the cluster configured, you're ready to deploy one of the components in your video rendering application. You decide to deploy a static version of your company's website to explore the Kubernetes deployment process.

Before discussing the Kubernetes way of deployment, let's review some of the steps you'd take to deploy a similar application to a non-Kubernetes environment.

Assume you're using an Azure virtual machine (VM) as your target platform. The first step is to prepare the server software to host the application. You will:

* Install the operating system
* Make sure to update the OS to the latest security and software patches
* Install and configure the web server software
* Deploy the web application

You'll repeat this process for each new VM when you decide to scale the website out to handle an increase in demand from customers.

An alternative approach is to run the website on a container-based platform like Azure Container Instances. You don't need to worry about the underlying server technology, but you'll have to configure and manage several containres to use this strategy manually.

Kubernetes and AKS help you orchestrate containers. The Kubernetes container orchestration features make it easy to manage workloads on the cluster. You deploy workloads by using containers bulit from container images to run your applications within an AKS cluster.

Here, you'll explore how to create workloads in your AKS cluster.

### Container Registry
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

![container-registry-diagram](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/4-1-container-registry-diagram.png)

Allows you to store container images safely in teh cloud for later deployment. You can think of the container registry as an archive that stores multiple versions of your container image. Each stored image has a tag assigned for identification.

For example, you might have the image `contoso-website:latest`, which would be a different version of the image with the tag `contoso-website:v1.0.0`.

Container registries might be public or private. Private registries require credentials to access and download images and will be the strategy you'll follow when you store container images.

Kubernetes only allows you to deploy images hosted in a container registry. Creating a private container registry will normally be part of your standard AKS deployment strategy.

### Kubernetes Pod
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Groups containers and applications into a logical structure. These pods hve no intelligence and are composed of one or more application containers. Each one has an IP address, network rules, and exposed ports.

For example, if you wanted to search all workloads releated to the `contoso-website`, you'd query the cluster for pods with the label `app` and the value `contoso-website`.

### Kubernetes Deployment
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

An evolution of pods, a deployment wraps the pods into an intelligent object that allows them to *scale out*. You can easily duplicate and scale your application to support more load without the need to configure complex networking rules.

Deployments allow users to update applications just by changing the image tag without downtime. When you update a deployment, instead of deleting all apps and creating new ones, the deployment turns off the online apps one by one and replaces them with the newest version. THis aspect means any deployment can update the pods inside it with no visible effect in availability.

#### Manifest Files
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Allows you to describe your workloads in the YAML format declaratively and simplify Kubernetes object management.

Imagine you have to deploy a workload by hand. You need to think about and manage several aspects. You'd need to create a container, select a specific node, wrap it in a pod, run the pod, monitor execution, and so on.

Manifest files contain all teh information that's needed to create and manage the described workload.

#### Kubernetes Label
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Allows you to logically group Kubernetes objects. These labels enable the system to query the cluster for objects that match a label with a specific name.

#### Manifest File Structure
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Differs depending on the type of resource that you create. However, manifest files share common instructions. These instructions define various aspects, such as the APIs to use and the type of workload to create.

The first two entries in all manifest files have two important keys, `apiVersion` and `kind`. Here's an example of a deployment file:

```yaml
apiVersion: apps/v1 # Where in teh API it resides
kind: Deployment #the kind of workload we're creating
```

Th `apiVersion` key defines the API server endpoint that manages the object you'll deploy.

The `kind` key dfine the workload this deployment will create.

Other common keys for all the files are the `metadata` and `name` keys. All Kubernetes resources *must* have a name. This name goes inside the `metadata` key.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: contoso-website # This will be the name of teh deployment.
```

#### Group Objects in a Deployment
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Deployments make use of a `label` to find and group pods. You define the label as part of your deployment's manifest file.

Here's an example. Notice the `matchLabels` value defined in the `selector` definition added to the `spec` definition.

```yaml
# deployment.yaml
# ...
spec:
  selector:
    matchLabels:
      app: contoso-website
# ...
```

From this point on, all files have different strucures based on what kind of resource you're telling Kubernetes to create.

#### Apply a Deployment File
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

You deploy a Kubernetes deployment manifest file by using `kubectl`:

```bash
kubectl apply -f ./deployment.yaml
```

## Deploy an App to AKS
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

In this exercise, you'll deploy your company's wbsite as a test app onto AKS. The website is a static website with an underlying technology stack of HTML, CSS, and JavaScript. It doesn't receive as many requests as the other services and provides us with a safe way to test deployment options.

### Create a Deployment Manifest
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)



### Apply the Manifest
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

