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
* [Networks in Kubernetes](#networks-in-kubernetes)
    * [Kubernetes Services](#kubernetes-services)
    * [Kubernetes Service Types](#kubernetes-service-types)
    * [Ingress](#ingress)
    * [Using Ingress Controllers](#using-ingress-controllers)
    * [Ingress Rules](#ingress-rules)
    * [Annotation](#annotation)
* [Enable Network Access to an Application](#enable-network-access-to-an-application)
    * [Create the Service Manifest](#create-the-service-manifest)
    * [Deploy the Service](#deploy-the-service)
    * [Create an Ingress Manifest](#create-an-ingress-manifest)
    * [Deploy the Ingress](#deploy-the-ingress)
* [Clean up Resources](#clean-up-resources)

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

    **Saved as [create-aks.sh](./assets/deploy-to-aks/create-aks.sh)**

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

    **Saved as [add-aks-nodepool.sh](./assets/deploy-to-aks/add-aks-nodepool.sh)**

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

    This command will add an entry to your `~/.kube/config` file (see [kube.config](./assets/deploy-to-aks/kube.config) for a sanitized example), which holds all the information to access your clusters. Kubectl enables you to manage multiple clusters from a single command-line interface.

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

Allows you to store container images safely in the cloud for later deployment. You can think of the container registry as an archive that stores multiple versions of your container image. Each stored image has a tag assigned for identification.

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

Manifest files contain all the information that's needed to create and manage the described workload.

#### Kubernetes Label
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Allows you to logically group Kubernetes objects. These labels enable the system to query the cluster for objects that match a label with a specific name.

#### Manifest File Structure
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Differs depending on the type of resource that you create. However, manifest files share common instructions. These instructions define various aspects, such as the APIs to use and the type of workload to create.

The first two entries in all manifest files have two important keys, `apiVersion` and `kind`. Here's an example of a deployment file:

```yaml
apiVersion: apps/v1 # Where in the API it resides
kind: Deployment #the kind of workload we're creating
```

Th `apiVersion` key defines the API server endpoint that manages the object you'll deploy.

The `kind` key dfine the workload this deployment will create.

Other common keys for all the files are the `metadata` and `name` keys. All Kubernetes resources *must* have a name. This name goes inside the `metadata` key.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: contoso-website # This will be the name of the deployment.
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

You create a deployment manifest file to deploy your application. The manifest file allows you to define what type of resource you want to deploy and all the details associated with the workload.

Kubernetes groups containers into logical structures called pods, which have no intelligence. Deployments add the missing intelligence to create your application. Let's create a deployment file.

1. Login to the Azure CLI

    ```bash
    az login
    ```

2. Create a manifest file ofr the Kubernetes deployment called [`deployment.yaml`](./assets/deploy-to-aks/deployment.yaml):

    ```bash
    touch ./deployment.yaml
    ```

3. Open the file in VS code:

    ```bash
    code ./deployment.yaml
    ```

4. Add the following code section of YAML:

    ```yaml
    apiVersion: apps/v1 # the API resource where this workload resides
    kind: Deployment # the kind of workload we're creating
    metadata:
        name: contoso-website # this will be the name of the deployment
    ```

    In this code, you added the first two keys to tell Kubernetes the `apiVersion` and `kind` of manifest you're creating. The `name` is the name of the deployment. You'll use it to identify and query the deployment information when you use `kubectl`.

5. A deployment wraps a pod. You make use of a template definition to define the pod information within the manifest file. The template is placed in the manifest file under the deployment specification section.

    Update the [`deployment.yaml`](./assets/deploy-to-aks/deployment.yaml) file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      template: # this is the teamplte of the pod inside the deployment
        metadata: # metadata for the pod
          labels:
            app: contoso-website
    ```

    Pods don't have given names when they're created inside deployments. The pod's name will be the deployment's name iwht a random ID added to the end.

    Notice the use of the `labels` key. You add the `labels` key to allow deployments to find and group pods.

6. A pod wraps one or more containers. All pods have a specification section that allows you to define the containers inside that pod.

    Update the [`deployment.yaml`](./assets/deploy-to-aks/deployment.yaml) file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      template:
        metadata:
          labels:
            app: contoso-website
        spec:
          containers: # here we define all containers
            - name: contoso-website
    ```

    the `containers` key is an array of container specifications because a pod can have one or more containers. The specification defines an `image`, a `name`, `resources`, `ports`, and other important information about the container.

    All running pods will follow the name `contoso-website-<UUID>`, where UUID is a generated ID to identify all resources uniquely.

7. It's a good practice to define a minimum and maximum amount of resources that the app is allowed to use from the cluster. You use the `resources` key to specify this information.

    Update the `deployment.yaml` file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      template:
        metadata:
          labels:
            app: contoso-website
        spec:
          containers:
            - image: mcr.microsoft.com/mslearn/samples/contoso-website
              name: contoso-website
              resources:
                requests: # minimum amount of resources requested
                  cpu: 100m
                  memory: 128Mi
                limits: # maximum amount of resources requested
                  cpu: 250m
                  memory: 256Mi
    ```

    Notice how the resoruce section allows you to specify the minimum resource amount as a request and the maximum resource amount as a limit.

8. The last step is to define the ports this container will expose externally through the `ports` key. The `ports` key is an array of objects, which means that a container in a pod can expose multiple ports with multiple names.

    Update the [`deployment.yaml`](./assets/deploy-to-aks/deployment.yaml) file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      template:
        metadata:
          labels:
            app: contoso-website
        spec:
          nodeSelector:
            kubernetes.io/os: linux
          containers:
            - image: mcr.microsoft.com/mslearn/samples/contoso-website
              name: contoso-website
              resources:
                requests:
                  cpu: 100m
                  memory: 128Mi
                limits:
                  cpu: 250m
                  memory: 256Mi
              ports:
                - containerPort: 80 # this container exposes port 80
                  name: http # we named the port "http" so we can refer to it later
    ```

    > In an AKS cluster which has multiple node pools (Linux and Windows), the deployment manifest file listed above defines a `nodeSelector` to tell your AKS cluster to run the sample application's pod on a node that can run Linux containers. Linux nodes can't run Windows containers and vice versa.

    Notice how you name the port by using the `name` key. Naming ports allows you to change the exposed port without changing files that reference that port.

9. Finally, add a selector section to define the workloads the deployment will manage. The `selector` key is placed inside the deployment specification section on the manifest file. Use the `matchLabels` key to list the labels fo rall the pods managed by the deployment.

    Update the [`deployment.yaml`](./assets/deploy-to-aks/deployment.yaml) file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      selector: # define the wrapping strategy
        matchLabels: # match all pods with the defined labels
          app: contoso-website # labels follow the `name: value` template
      template:
        metadata:
          labels:
            app: contoso-website
        spec:
          nodeSelector:
            kubernetes.io/os: linux
          containers:
            - image: mcr.microsoft.com/mslearn/samples/contoso-website
              name: contoso-website
              resources:
                requests:
                  cpu: 100m
                  memory: 128Mi
                limits:
                  cpu: 250m
                  memory: 256Mi
              ports:
                - containerPort: 80
                  name: http
    ```

### Apply the Manifest
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

1. In the terminal, run the `kubectl apply` command to submit the deployment manifest to your cluster:

    ```bash
    kubectl apply -f ./deployment.yaml
    ```

    The command should output a result similar to the following example:

    ```
    deployment.apps/contoso-website created
    ```

2. Run the `kubectl get deploy` command to check if the deployment was successful:

    ```bash
    kubectl get deploy contoso-website
    ```

    The command should output a table similar to the following example:

    NAME | READY | UP-TO-DATE | AVAILABLE | AGE
    -----|-------|------------|-----------|----
    contoso-website | 0/1 | 1 | 0 | 16s

3. Run the `kubectl get pods` command to check if the pod is running.

    The command should output a table similar to the following example:

    NAME | READY | STATUS | RESTARTS | AGE
    -----|-------|--------|----------|----
    contoso-website-{uuid} | 1/1 | Running | 0 | 2m5s

## Networks in Kubernetes
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

An AKS cluster blocks all inbound traffic from the internet to the cluster to assure network security. Deployed workloads in Kubernetes are, by default, only accessible from inside the cluster. To expose applications to the outside world, you need to open specific ports and forward them to your services.

The configuration of ports and port forwarding in Kubernetes is different from what you might be used to in other environments. On a virtual machine (VM), you'll configure the OS-level firewall to allow inbound traffic to port 443 and allow HTTPS web traffic. In Kubernetes, the control plane manages network configuration based on declarative instructions you provide.

The network configuration for containers is temporary. A container's configuration and the data in it isn't persistent between executions. After you delete a container, all information is gone unless it's configured to use a volume. The same applies to the container's network configuration and any IP addresses assigned to it.

A deployment is a logical grouping of pods. It isn't considered a physical workload and isn't assigned an IP address. But each pod is automatically assigned an IP address. When the pod is destroyed, the IP address is lost. This behavior makes a manual network configuration strategy complex.

Kubernetes has two network availability abstractions that allow you to expose any app without worrying about the underlying infrastructure or assigned pod IP addresses.

These abstractions are the *services* and *ingresses*. They're both responsible for allowing and redirecting the traffic from external sources to the cluster.

### Kubernetes Services
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

![service-diagram](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/6-1-service-diagram.png)

A Kubernetes service is a workload that abstracts the IP address for networked workloads. A Kubernetes service acts as a load balancer and redirects traffic to the specified ports by using port-forwarding rules.

You define a service in the same way as a deployment, by using a YAML manifest file. The service uses the same `selector` key as deployments to select and group resources with matching labels into one single IP.

A Kubernetes service needs four pieces of information to route traffic:

Information | Description
------------|------------
Target resource | The target resource is defined by the `selector` key in the service manifest file. This value selects all resources with a given label onto a single IP address.
Service port | This port is the inbound port for your application. All the requests come to this port from where the service forwards the requests to the resource.
Network protocol | This value identifies the network protocol for which the service will forward network data.
Resource port | This value identifies the port on the target resource on which incoming requests are received. This port is defined by the `targetPort` key in the service manfiest file.

### Kubernetes Service Types
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Service cna be of several types. Each type changes the behavior of the applications selected by the service.

* **ClusterIP:** This value exposes the applications internally only. This option allows the service to act as a port-forwarder and makes the service available within the cluster. This value is the default when omitted.
* **NodePort:** This value exposes the service externally. It assigns each node a static port that responds to that service. When accessed through `nodeIp:port`, the node automatically redirects the request to an internal service of the `ClusterIP` type. This service then forwards the request to the applications.
* **LoadBalancer:** This value exposes the service externally by using Azure's load-balancing solution. When created, this resource spins up an Azure Load Balancer resource within your Azure subscription. Also, this type automatically creates a `NodePort` service to which the load balancer's traffic is redirected and a `ClusterIP` service to forward it internally.
* **ExternalName:** This value maps the service by using a DNS resolution through a CNAME record. You use this service type to direct traffic to services that exist outside the Kubernetes cluster.

### Ingress
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

![ingress-diagram](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/6-2-ingress-diagram.png)

Ingress exposes routes for HTTP and HTTPS traffic from outside a cluster to services inside the cluster. You define ingress routes by using *ingress rules*. A Kubernetes cluster rejects all incoming traffic wihtout these routes defined.

Assume you want to allow clients to access your website through the `http://contoso.com` web address. For a client to access your app inside the cluster, the cluster must respond to the website's CNAME and route the requests to the relevant pod.

#### Using Ingress Controllers
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Kubernetes uses ingress controllers to manage the configuration of ingresses in a cluster and provides several features. An ingress controller:

* Acts as a reverse proxy to allow external URLs
* Might act as a load balancer
* Terminates SSL/TLS requests
* Offers name-based virtual hosting

In AKS, the ingress controller links to a *DNS Zone* resource in your Azure subscription. The DNS Zone is automatically created as part of the cluster creation porcess on your behalf. The link makes it possible for the cluster to automatically generate a zone record that points the DNS name to the exposed application's IP address and port.

In AKS, the HTTP application routing add-on allows you to create ingress controllers.

#### Ingress Rules
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

Ingress rules define where traffic is coming from and where to direct it within a cluster. You define ingress rules in an ingress deployment manifest file.

These rules are defined in the `rules` key of the manifest file. Each rule is a set of values that describes the rule.

For example, assume you want to allow clients to access your website by using the URL `http://example.com/site`. You want to route traffic to your video rendering service website. Here's an example of the defined ingress rule to allow this behavior:

```yaml
rules:
  - host: example.com # a FQDN that describes the host where that rule should be applied
    http:
      paths: # a list of paths and handlers for the host
        - path: /site # which path is this rule referring to
          backend: # how the ingress will handle the requests
            serviceName: contoso-website # which service the request will be forwarded to
            servicePort: 80 # which port in that service
```

This example defines a rule that allows all traffic using the address `example.com` and path `/site` to enter the cluster. This traffic is then routed to the `contoso-website` service on port `80`.

#### Annotation
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

An annotation allows you to attach non-identifying metadata, such as ingress configurations, for workloads. You can think of the annotation as an internal label that defines specific configurations for resources. For example, you might want to use a specific ingress controller that supports name rewriting or payload limiting.

Here's an example of the annotation in a manifest file that specifies the use of the HTTP application routing add-on.

```yaml
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: contoso-website
  annotations:
    kubernetes.io/ingress.class: addon-http-application-routing # use the HTTP application routing add-on
```

## Enable Network Access to an Application
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

You successfully deployed the video rendering service website to your cluster. But you noticed that you couldn't access the website from any client external to the cluster. The problem is that you haven't exposed your application to the internet yet. By default, Kubernetes blocks all external traffic. You'll need to add an *ingress rule* to allow traffic into the cluster.

### Create the Service Manifest
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

1. Login to the Azure CLI

    ```bash
    az login --use-device-code
    ```

2. Create a manifest file for the Kubernetes service called [`service.yaml`](./assets/deploy-to-aks/service.yaml):

    ```bash
    touch service.yaml
    ```

3. Open [`service.yaml`](./assets/deploy-to-aks/service.yaml) in VS Code and add the following:

    ```bash
    code ./service.yaml
    ```

    ```yaml
    apiVersion: v1
    kind: Service
    metadata:
      name: contoso-website
    ```

    In this code, you added the first two keys to tell Kubernetes the `apiVersion` and `kind` of manifest you're creating. The `name` is the name of the service. You'll use it to identify and query the service information when you use `kubectl`.

4. You define how the service will behave in the specification section of the manifest file. The first behavior you need to add is the type of service. Set the `type` key to `clusterIP`.

    Update the [`service.yaml`](./assets/deploy-to-aks/service.yaml) file to match the following:

    ```yaml
    apiVersion: v1
    kind: Service
    metadata:
      name: contoso-website
    spec:
      type: ClusterIP
    ```

5. You define the pods the service will group and provide coverage by adding a `selector` section to the manifest file. Add the `selector`, and set the `app` key value to the `contoso-website` label of your pods as specified in your earlier deployment's manifest file.

    Update the [`service.yaml`](./assets/deploy-to-aks/service.yaml) file to match the following:

    ```yaml
    apiVersion: v1
    kind: Servie
    metadata:
      name: contoso-website
    spec:
      type: ClusterIP
      selector:
        app: contoso-website
    ```

6. You define the port-forwarding rules by adding a `ports` section to the manifest file. The service must accept all TCP requests on port 80 and forward the request to the HTTP target port for all pods matching the selector value defined earlier.

    Update the `service.yaml` file to match the following:

    ```yaml
    apiVersion: v1
    kind: Service
    metadata:
      name: contoso-website
    spec:
      type: ClusterIP
      selector:
        app: contoso-website
      ports:
        - port: 80 # SERVICE exposed port
          name: http # SERVICE port name
          protocol: TCP # the protocol the SERVICE will listen to
          targetPort: http # port to forward to in the POD
    ```

### Deploy the Service
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

1. Run the `kubectl apply` command to submit thet service manifest to your cluster:

    ```bash
    kubectl apply -f ./service.yaml
    ```

    The command should output a result similar to the following example:

    ```
    service/contoso-website created
    ```

2. Run the `kubectl get service` command to check if the deployment was successful:

    ```bash
    kubectl get service contoso-website
    ```

    The command should output a result similar to the following example. Make sure the column `CLUSTER-IP` is filled with an IP address and the column `EXTERNAL-IP` is `<none>`. Also, make sure the column `PORT(S)` is defined to `80/TCP`:

    NAME | TYPE | CLUSTER-IP | EXTERNAL-IP | PORT(S) | AGE
    -----|------|------------|-------------|---------|----
    contoso-website | ClusterIP | 10.0.113.130 | \<none\> | 80/TCP | 16s

    With the external IP set to `<none>`, the application isn't available to external clients. The service is only available to the internal cluster.

### Create an Ingress Manifest
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

To expose the website to the world via DNS, you must create an ingress controller.

1. Create a manifest file for the Kubernetes service called [`ingress.yaml`](./assets/deploy-to-aks/ingress.yaml):

    ```bash
    touch ingress.yaml
    ```

2. Open the [`ingress.yaml`](./assets/deploy-to-aks/ingress.yaml) file in code and add the following section:

    ```bash
    code ./ingress.yaml
    ```

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: contoso-website
    ```

    In this code, you added the first two keys to tell Kubernetes the `apiVersion` and `kind` of manifest you're creating. The `name` is the name of the ingress. You'll use it to identify and query the ingress information when you use `kubectl`.

3. Create an `annotations` key inside the `metadata` section of the manifest file called to use the HTTP application routing add-on for this ingress. Set the key to `kubernetes.io/ingress.class` and a value of `addon-http-application-routing`.

    Update the [`ingress.yaml`](./assets/deploy-to-aks/ingress.yaml) file to match the following:

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: contoso-website
      annotations:
        kubernetes.io/ingress.class: addon-http-application-routing
    ```

4. Set the FQDN of the host allowed access to the cluster.

    Run the `az network dns zone list` command to query the Azure DNS zone list:

    **Saved as [`query-dns-zone.sh`](./assets/deploy-to-aks/query-dns-zone.sh)**

    ```bash
    az aks show \
      -g $RESOURCE_GROUP \
      -n $CLUSTER_NAME \
      -o tsv \
      --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
    ```

5. Copy the output and update the [`ingress.yaml`](./assets/deploy-to-aks/ingress.yaml) file to match the following:

    > Replace the `<zone-name>` placeholder value with the `ZoneName` value copied from above.

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: contoso-website
      annotations:
        kubernetes.io/ingress.class: addon-http-application-routing
    spec:
      rules:
        - host: contoso.<zone-name> # which host is allowed to enter the cluster
    ```

6. Add the back-end configuration to your ingress rule. Create a key named `http` and allow the `http` protocol to pass through. Then, define the `paths` key that will allow you to filter whether this rule applies to all paths or the website or only some of them.

    Update the [`ingress.yaml`](./assets/deploy-to-aks/ingress.yaml) file to match the following:

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: contoso-website
      annotations:
        kubernetes.io/ingress.class: addon-http-application-routing
    spec:
      rules:
        - host: contoso.<zone-name> # which host is allowed to enter the cluster
          http:
            paths:
              - backend: # how the ingress will handle the requests
                  service:
                    name: contoso-website #which service the request will be forwarded to
                    port:
                      name: http # which port in that service
                path: / # which path is this rule referring to
                pathType: Prefix # see more at https://kubernetes.io/docs/concepts/services-networking/ingress/#path-types
    ```

### Deploy the Ingress
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

1. Run the `kubectl apply` command to submit the ingress manifest to your cluster:

    ```bash
    kubectl apply -f ./ingress.yaml
    ```

    The command should output a result similar to the following:

    ```
    ingress.extensions/contoso-website created
    ```

2. Run the `kubectl get ingress` command to check if the deployment was successful:

    ```bash
    kubectl get ingress contoso-website
    ```

    The command should output a result similar to the following example:

    NAME | CLASS | HOSTS | ADDRESS | PORTS | AGE
    -----|-------|-------|---------|-------|----
    contoso-website | \<none\> | contoso.<zone-name>.aksapp.io | 4.236.210.21 | 80 | 36s

    Make sure the `ADRESS` column of the output is filled with an IP address. That's the address of your cluster.

    > There can be a delay between the creation of the ingress and the creation of the zone record. It can take up to five minutes for zone records to propogate.

3. Open your browser, and go to the FQDN described in the output. You should see a website that looks like the following example screenshot:

    ![contoso-website](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/7-website-success.png)

## Clean Up Resources
[Back to Top](#deploy-a-containerized-application-on-azure-kubernetes-service)

1. Open the [Azure Portal](https://portal.azure.com).

2. Select **Resource groups** on the left.

3. Find the **rg-contoso-video** resource group, or the resource group name you used, and select it.

4. On the **Overview** tab of the resource group, select **Delete resource group**.

5. Enter the name of the resource group to config. Select **Delete** to delete all of the resources you created in this module.

6. Finally, run the `kubectl config delete-context` command to remove the deleted clusters context. Here's an example of the complete command> Remember to replace the name of the cluster with your cluster's name:

    ```bash
    kubectl config delete-context aks-contoso-video
    ```

    If successful, the command returns the following:

    ```
    deleted context aks-contoso-video from /home/user/.kube/config
    ```