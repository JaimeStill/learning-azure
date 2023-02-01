# Azure Kubernetes Service

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
[Back to Top](#azure-kubernetes-service)

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

## Kubernetes Clusters
[Back to Top](#azure-kubernetes-service)

Kubernetes is based on clusters. Instead of haivng a single virtual machine (VM), it uses several machines working as one. These VMs are called nodes. Kubernetes is a cluster-based orchestrator. It provides your application with several benefits, such as availability, monitoring, scaling, and rolling updates.

## Cluster Nodes
[Back to Top](#azure-kubernetes-service)

A cluster is node-based. There are two types of nodes in a Kubernetes cluster that provide specific functionality.

* **Control plane nodes:** These nodes host the cluster's control plane aspects and are reserved for services that control the cluster. They're responsible for providing the API you and all the other nodes use to communicate. No workloads are deployed or scheduled on these nodes.

* **Nodes:** These nodes are responsible for executing custom workloads and applications, such as components from a cloud-based video rendering service.

## Cluster Architectures
[Back to Top](#azure-kubernetes-service)

A cluster architecture allows you to conceptualize the number of control planes and nodes you'll deploy in your Kubernetes cluster.

For example, the number of nodes in a cluster should always be more than two. When a node becomes unavailable, the Kubernetes scheduler will try to reschedule all the workloads running on this node onto the remaining nodes in the cluster.

There are two popular cluster architectures for Kubernetes-based deployments:

* Single control plane and multiple nodes
* Single control plane and a single node

### Single Control Plane and Multiple Nodes
[Back to Top](#azure-kubernetes-service)

![single-cp-multiple-nodes](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/2-1-diagram.png)

This architecture is the most common architectural pattern, and is the easiest to deploy, but it doesn't provide high availability to your cluster's core management services.

If the control plane node becomes unavailable for some reason, no other interaction can happen with the cluster. This is the case even by you as an operator, or by any workloads that user Kubernetes' APIs to communicate until, at least, the API server is back online.

Despite being less available than others, this architecture should be enough for most situations. It's less likely that the core management services become unavailable compared to a node going offline. The control plane nodes are subject to fewer modifications than nodes and are more resilient.

If you're dealing with a produciton scenario, this architecture might not be the best solution.

### Single Control Plane and a Single Node
[Back to Top](#azure-kubernetes-service)

![single-cp-single-node](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/2-2-single-diagram.png)

This architecture is a variant of the previous architcture and is used in development environments. This architecture provides only one node that hosts both the control plane and a worker node. It's useful when testing or experimenting with different Kubernetes concepts. The single control plane and single node architecture limits cluster scaling and makes this architecture unsuitable for production and staging use.

## AKS Cluster Configuration
[Back to Top](#azure-kubernetes-service)

When you create a new AKS cluster, you have different items of information that you need to configure. Each item affects the final configuration of your cluster.

These items include:
* Node pools
* Node count
* Automatic routing

### Node Pools
[Back to Top](#azure-kubernetes-service)

![node-pool-diagram](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/2-3-node-pool-diagram.png)

You create *node pools* to group nodes in your AKS cluster. When you create a node pool, you specify the VM size and OS type (Linux or Windows) for each node in the node pool based on application requirement. To host user application pods, node pool **Mode** should be **User** otherwise **System**.

By default, an AKS cluster will have a Linux node pool (**System Mode**) whether it's created through Azure portal or CLI. However, you'll always have an option to add Windows node pools along with default Linux node pools durin the creation wizard in the portal, via CLI, or in ARM templates.

Node pools use Virtual Machine Scale Sets as the underlying infrastructure to allow the cluster to scale the number of nodes in a node pool. New nodes created in the node pool will always be the same size as you specified when you created the node pool.A node pool describes a group of nodes with the same configuration in an AKS cluster. These nodes contain the underlying VMs that run your applications. You can create two types of node pools on an AKS-managed Kubernetes cluster:

* System node pools
* User node pools

### Node Count
[Back to Top](#azure-kubernetes-service)

The number of nodes your cluster will have in a node pool. Nodes are Azure VMs. You can change their size and count to match your usage pattern.

You can change the node count later in the cluster's configuration panel. It's also a best practice to keep this number as low as possible to avoid unnecessary costs and unused compute power.

### Automatic Routing
[Back to Top](#azure-kubernetes-service)

A Kubernetes cluster blocks all external communications by default. For example, assume you deploy a website that's accessible to all clients. You need to manually create an *ingress* with an exception that allows incoming client connections to that particular service. This configuration requires network-related changes that forward requests from the client to an internal IP on the cluster, and finally to your application. Depending on your specific requirements, this process can be complicated.

AKS allows you to overcome the complexity by enabling what's called HTTP application routing. This add-on makes it easy to access applications on the cluster through an automatically deployed ingress controller.

### System Node Pools
[Back to Top](#azure-kubernetes-service)

System node pools host critical system pods that make up the control plane of your cluster. A system node pool allows the use of Linux only as the node OS and runs only Linux-based workloads. Nodes in a system node pool are reserved for system workloads and normally not used to run custom workloads. Every AKS cluster must contain at least one system node pool with at least one node, and you must define the underlying VM sizes for nodes.

### User Node Pools
[Back to Top](#azure-kubernetes-service)

User node pools support your workloads, and you can specify Windows or Linux as the node operating system. You can also define the underlying VM sizes for nodes and run specific workloads. For example, a solution has a batch-processing service that you deploy to a node pool configured with general-purpose VMs. The new predictive-modeling service requires higher-capacity, GPU-based VMs. You decide to configure a separate node pool and configure it to use GPU-enabled nodes.

### Number of Nodes in a Node Pool
[Back to Top](#azure-kubernetes-service)

You can configure up to 100 nodes in a node pool. However, the number of nodes you choose to configure depends on the number of pods that run per node.

For example, in a system node pool, it's essential to set the maximum number of pods that run on a single node to 30. This value guarantees that enough space is available to run the system pods critical to cluster health. When the number of pods exceeds this minimum value, new nodes are required in the pool to schedule additional workloads. For this reason, a system node pool needs at least one node in the pool. For production environments, the recommended node count for a system node pool is a minimum of three nodes.

User node pools are designed to run custom workloads and don't have the 30-pod requirement. User node pools allow you to set the node count for a pool to zero.

### Ingress Controllers
[Back to Top](#azure-kubernetes-service)

![http-app-routing-diagrm](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/2-4-http-application-routing-diagram.png)

Ingress controllers provide the capability to deploy and expose your applications to the world without the need to configure network-related services.

Ingress controllers create a reverse-proxy server that automatically allows for all the requests to be servved from a single DNS output. You don't have to create a DNS record every time a new service is deployed. The ingress controller will take care of it. When a new ingress is deployed to the cluster, the ingress controller creates a new record on an Azure managed DNS zone and links it to an existing load balancer. This functionality allows for easy to access to the resource through the internet without the need for additional configuration.

Despite the advantages, HTTP application routing is better suited to more basic clusters. It doesn't provide the amount of customization needed for a more complex configuration. If you plan to deal with more complex clusters, there are better-suited options like the official Kubernetes ingress controller.

### Container Registry
[Back to Top](#azure-kubernetes-service)

![container-registry-diagram](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/4-1-container-registry-diagram.png)

Allows you to store container images safely in the cloud for later deployment. You can think of the container registry as an archive that stores multiple versions of your container image. Each stored image has a tag assigned for identification.

For example, you might have the image `contoso-website:latest`, which would be a different version of the image with the tag `contoso-website:v1.0.0`.

Container registries might be public or private. Private registries require credentials to access and download images and will be the strategy you'll follow when you store container images.

Kubernetes only allows you to deploy images hosted in a container registry. Creating a private container registry will normally be part of your standard AKS deployment strategy.

### Kubernetes Pod
[Back to Top](#azure-kubernetes-service)

Groups containers and applications into a logical structure. These pods hve no intelligence and are composed of one or more application containers. Each one has an IP address, network rules, and exposed ports.

For example, if you wanted to search all workloads releated to the `contoso-website`, you'd query the cluster for pods with the label `app` and the value `contoso-website`.

### Kubernetes Deployment
[Back to Top](#azure-kubernetes-service)

An evolution of pods, a deployment wraps the pods into an intelligent object that allows them to *scale out*. You can easily duplicate and scale your application to support more load without the need to configure complex networking rules.

Deployments allow users to update applications just by changing the image tag without downtime. When you update a deployment, instead of deleting all apps and creating new ones, the deployment turns off the online apps one by one and replaces them with the newest version. THis aspect means any deployment can update the pods inside it with no visible effect in availability.

### Manifest Files
[Back to Top](#azure-kubernetes-service)

Allows you to describe your workloads in the YAML format declaratively and simplify Kubernetes object management.

Imagine you have to deploy a workload by hand. You need to think about and manage several aspects. You'd need to create a container, select a specific node, wrap it in a pod, run the pod, monitor execution, and so on.

Manifest files contain all the information that's needed to create and manage the described workload.

### Kubernetes Label
[Back to Top](#azure-kubernetes-service)

Allows you to logically group Kubernetes objects. These labels enable the system to query the cluster for objects that match a label with a specific name.

### Manifest File Structure
[Back to Top](#azure-kubernetes-service)

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

### Group Objects in a Deployment
[Back to Top](#azure-kubernetes-service)

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

### Apply a Deployment File
[Back to Top](#azure-kubernetes-service)

You deploy a Kubernetes deployment manifest file by using `kubectl`:

```bash
kubectl apply -f ./deployment.yaml
```

## Networks in Kubernetes
[Back to Top](#azure-kubernetes-service)

An AKS cluster blocks all inbound traffic from the internet to the cluster to assure network security. Deployed workloads in Kubernetes are, by default, only accessible from inside the cluster. To expose applications to the outside world, you need to open specific ports and forward them to your services.

The configuration of ports and port forwarding in Kubernetes is different from what you might be used to in other environments. On a virtual machine (VM), you'll configure the OS-level firewall to allow inbound traffic to port 443 and allow HTTPS web traffic. In Kubernetes, the control plane manages network configuration based on declarative instructions you provide.

The network configuration for containers is temporary. A container's configuration and the data in it isn't persistent between executions. After you delete a container, all information is gone unless it's configured to use a volume. The same applies to the container's network configuration and any IP addresses assigned to it.

A deployment is a logical grouping of pods. It isn't considered a physical workload and isn't assigned an IP address. But each pod is automatically assigned an IP address. When the pod is destroyed, the IP address is lost. This behavior makes a manual network configuration strategy complex.

Kubernetes has two network availability abstractions that allow you to expose any app without worrying about the underlying infrastructure or assigned pod IP addresses.

These abstractions are the *services* and *ingresses*. They're both responsible for allowing and redirecting the traffic from external sources to the cluster.

### Kubernetes Services
[Back to Top](#azure-kubernetes-service)

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
[Back to Top](#azure-kubernetes-service)

Service cna be of several types. Each type changes the behavior of the applications selected by the service.

* **ClusterIP:** This value exposes the applications internally only. This option allows the service to act as a port-forwarder and makes the service available within the cluster. This value is the default when omitted.
* **NodePort:** This value exposes the service externally. It assigns each node a static port that responds to that service. When accessed through `nodeIp:port`, the node automatically redirects the request to an internal service of the `ClusterIP` type. This service then forwards the request to the applications.
* **LoadBalancer:** This value exposes the service externally by using Azure's load-balancing solution. When created, this resource spins up an Azure Load Balancer resource within your Azure subscription. Also, this type automatically creates a `NodePort` service to which the load balancer's traffic is redirected and a `ClusterIP` service to forward it internally.
* **ExternalName:** This value maps the service by using a DNS resolution through a CNAME record. You use this service type to direct traffic to services that exist outside the Kubernetes cluster.

### Ingress
[Back to Top](#azure-kubernetes-service)

![ingress-diagram](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/6-2-ingress-diagram.png)

Ingress exposes routes for HTTP and HTTPS traffic from outside a cluster to services inside the cluster. You define ingress routes by using *ingress rules*. A Kubernetes cluster rejects all incoming traffic wihtout these routes defined.

Assume you want to allow clients to access your website through the `http://contoso.com` web address. For a client to access your app inside the cluster, the cluster must respond to the website's CNAME and route the requests to the relevant pod.

#### Using Ingress Controllers
[Back to Top](#azure-kubernetes-service)

Kubernetes uses ingress controllers to manage the configuration of ingresses in a cluster and provides several features. An ingress controller:

* Acts as a reverse proxy to allow external URLs
* Might act as a load balancer
* Terminates SSL/TLS requests
* Offers name-based virtual hosting

In AKS, the ingress controller links to a *DNS Zone* resource in your Azure subscription. The DNS Zone is automatically created as part of the cluster creation porcess on your behalf. The link makes it possible for the cluster to automatically generate a zone record that points the DNS name to the exposed application's IP address and port.

In AKS, the HTTP application routing add-on allows you to create ingress controllers.

#### Ingress Rules
[Back to Top](#azure-kubernetes-service)

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
[Back to Top](#azure-kubernetes-service)

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

## Manage Application Demand in an AKS Cluster
[Back to Top](#azure-kubernetes-service)

The function in AKS that provides for increasing or decreasing the amount of compute resources in a Kubernetes cluster is called *scaling*. You scale either the number of workload instances that need to run or the number of nodes on which these workloads run. You scale workloads on an AKS-managed cluster in one of two ways. The first option is to scale the pods or nodes manually as necessary. Or, you can use the horizontal pod autoscaler to scale pods and the cluster autoscaler to scale the nodes.

### Scaling a Node Pool Manually
[Back to Top](#azure-kubernetes-service)

If you're running workloads that execute for a specific duration at specific intervals, manually scaling the node pool size is a way to control node costs.

Assume that the new compute-heavy predictive-modeling service requires a GPU-based node pool and runs only at specific intervals. You can configure the node pool with specific GPU-based nodes and scale the node pool to zero nodes when you're not using the cluster.

Here's an example of the `az aks node pool add` command that you can use to create the node pool. Notice the `--node-vm-size` parameter, which specifies the `Standard_NC6` GPU-based VM size for the nodes in the pool:

```bash
az aks node pool add \
    --resource-group resourceGroup \
    --cluster-name aksCluster \
    --name gpunodepool \
    --node-count 1 \
    --node-vm-size Standard_NC6 \
    --no-wait
```

When the pool is ready, you can use the `az aks nodepool scale` command to scale the node pool to zero nodes. Notice that the `--node-count` parameter is set to zero. Here's an example of the command:

```bash
az aks nodepool scale \
    --resource-group resourceGroup \
    --clusterName aksCluster \
    --name gpunodepool \
    --node-count 0
```

### Scaling a Cluster Automatically
[Back to Top](#azure-kubernetes-service)

![cluster-autoscaler](https://learn.microsoft.com/en-us/training/modules/aks-optimize-compute-costs/media/2-cluster-autoscaler.png)

AKS uses the Kubernetes cluster autoscaler to automatically scale workloads. The cluster can scale by using two options:

* The horizontal pod autoscaler
* The cluster autoscaler

#### Horizontal Pod Autoscaler
[Back to Top](#azure-kubernetes-service)

Use the Kubernetes horizontal pod autoscaler to monitor the resource demand on a cluster and automatically scale the number of workload replicas.

The Kubernetes Metrics Server collects memory and processor metrics from controllers, nodes, and containers that run on the AKS cluster. One way to access this information is to use the Metrics API. The horizontal pod autoscaler checks the Metrics API every 30 seconds to decide whether your application needs additional instances to meet the required demand.

Assume your company also has a batch-processing service that schedules drone flight paths. You see the service gets inundated with requests and builds up a backlog of deliveries, causing delays and frustrations for customers. Increasing the number of batch-processing service replicas will enable the timely processing of orders.

To solve the problem, you configure the horizontal pod autoscaler to scale up the number of service replicas when needed. When batch requests decrease, it scales the replica count down.

However, the horizontal pod autoscaler scales pods only on available nodes in the configured node pools of the cluster.

#### Cluster Autoscaler
[Back to Top](#azure-kubernetes-service)

A resource constraint is triggered when the horizontal pod autoscaler can't schedule additional pods on the existing nodes in a node pool. You use the cluster autoscaler to scale the number of nodes in a cluster's node pools. The cluster autoscaler checks the defined metrics and scales the number of nodes up or down based on the computing resources required.

The cluster autoscaler is used alongside the horizontal pod autoscaler.

The cluster autoscaler monitors for both scale-up and scale-down events, and allows the Kubernetes cluster to change the node count ina node pool as resource demands change.

You configure each node pool with different scale rules. For example, you might want to configure only one node pool to allow autoscaling. Or you might configure a node pool to scale only to a specific number of nodes.

## Spot VM
[Back to Top](#azure-kubernetes-service)

A *spot virtual machine* is a VM that gives you access to unused Azure compute capacity at deep discounts. Spot VMs replace the existing, low-priority VMs in Azure. You can use spot VMs to run workloads that include:

* High-performance computing scenarios, batch processing, or visual-rendering applications.
* Large-scale, stateless applications
* Developer/test environments, including continuous integration (CI) and continuous delivery (CD) workloads.

### Spot VM Availability
[Back to Top](#azure-kubernetes-service)

Spot VM availability depends on factors such as capacity, size, region, and time of day. Azure allocates VMs only if capacity is available. As a result, there's no service-level agreement (SLA) for these types of VMs and they offer no high-availability guarantees.

### Spot VM Eviction Policy
[Back to Top](#azure-kubernetes-service)

The default eviction policy for spot VMs is **Deallocate**. Azure will evict spot VMs with 30 seconds of notice when capacity in a region becomes limited. A VM that's set with the **Deallocate** policy moves to the stopped-deallocated state when evicted. You can redeploy an evicted VM when spot capacity becomes available again. A deallocated VM is still counted toward your spot virtual CPU (vCPU) quota, and charges for the underlying allocated disks still apply.

### Spot VM Scale Set
[Back to Top](#azure-kubernetes-service)

A spot virtual machine scale set is a virtual machine scale set that supports Azure spot VMs. These VMs behave the same way as normal spot VMs, but with one difference: when you use virtual machine scale set support for spot VMs in Azure, you choose between two eviction policies:

* **Deallocate**: The Deallocate policy functions exactly as described above.
* **Delete**: The Delete policy allows you to avoid the cost of disks and hitting quota limits. With the Delete eviction policy, evicted VMs are deleted together with their underlying idsks. The scale set's autoscaling feature can now automatically try to compensate for the eviction of VMs by creating new VMs. Although the creation of VMs isn't guaranteed, the evicted VMs don't count toward your vCPU quota or incur costs for underlying disks.

A best practice is to use the autoscale feature only when you set the eviction policy to **Delete** on the scale set.

## Spot Node Pool
[Back to Top](#azure-kubernetes-service)

A *spot node pool* is a user node pool that uses a spot virtual machine scale set. AKS supports spot VMs when you:

* Need to create user node pools.
* Want the cost benefits offered by virtual machine scale set support for Azure spot VMs.

Use spot node pools to:

* Take advantatge of unused capacity in Azure.
* Use scale set features with the Delete eviction policy.
* Define the maximum price you want to pay per hour.
* Enable the recommended AKS Kubernetes cluster autoscaler when using spot node pools.

For example, to support a batch-processing, you can create a spot user node pool and enable the cluster autoscaler. You the nconfigure the horizontal pod scaler to deploy additional batch-processing services to match resource demands.

As the demand for nodes increases, the cluster autoscaler can scale the number of nodes up and down in the spot node pool. If node evictions happen, the cluster autoscaler keeps trying to scale up the node count if additional nodes are still needed.

### Spot Pool Limitations
[Back to Top](#azure-kubernetes-service)

Before you decide to add a spot user node pool to your AKS cluster, consider the following limitations:

* The underlying spot scale set is deployed only to a single fault domain and offers no high-availability guarantees.
* The AKS cluster needs multiple node-pool support to be enabled.
* You can use spot node pools only as user node pools.
* Spot node pools can't be upgraded.
* The creation of spot VMs isn't guaranteed. The creation of spot nodes depends on capacity and quota availability in the cluster's deployed Azure region.

Remember tha tspot node pools should be used only for workloads that can be interrupted.

## Add a Spot Node Pool to an AKS Cluster
[Back to Top](#azure-kubernetes-service)

A spot node pool can't be a system node pool for an AKS cluster. You'll first create your cluster and then use the `az aks nodepool add` command to add a new user node pool.

You set several parameters for a new node pool to configure it as a spot node pool.

**Example**  

```bash
az aks nodepool add \
    --resource-group resourceGroup \
    --cluster-name aksCluster \
    -- name spotpool01 \
    --enable-cluster-autoscaler \
    --max-count 3 \
    --min-count 1 \
    --priority Spot \
    --eviction-policy Delete \
    --spot-max-price -1 \
    --no-wait
```

### Priority
[Back to Top](#azure-kubernetes-service)

The `--priority` parameter is set to `Regular` by default for a new node pool. Set th evalue to `Spot` to indicate that the new pool you're creating is a spot node pool. This value can't be changed after creation.

### Eviction Policy
[Back to Top](#azure-kubernetes-service)

A spot node pool must use a virtual machine scale set. The spot node pool uses a spot scale set. Set `--eviction-policy` to `Delete` to allow the scale set to remove both the node and the underlying, allocated disk used by the node. This value can't be changed after creation.

You can set the eviction policy to `Deallocate`; however, when these nodes are evicted, they'll count against your compute quota and impact later scaling or upgrading of the cluster.

### Maximum Price for Spot Node
[Back to Top](#azure-kubernetes-service)

Spot node pools enable you to optimize costs by setting the maximum amount that you're willing to pay per spot node per hour. To set your safe ammount, use the `--spot-max-price` parameter. Newly created spot nodes are evicted when this value is reached. You can set this value to any positive amount up to five decimal places, or set it to -1. Setting the `--spot-max-price` value to -1 affects your node pool in the following ways:

* Nodes won't be evicted based on the node's price.
* The cost for new nodes will be the current price for spot nodes or the price for a standard node, whichever is lower

For example, if you set the value to 0.98765, the maximum price for a node will be USD0.98765 per hour. When the node's consuption exceeds this amount, it's evicted.

### Enable the Cluster Autoscaler
[Back to Top](#azure-kubernetes-service)

We recommend that you enable the cluster autoscaler by using the `--enable-cluster-autoscaler` parameter. If you don't use the cluster autoscaler, you risk the node count dropping to zero in the node pool as nodes are evicted because of Azure capacity constraints.

### Minimum Node Count
[Back to Top](#azure-kubernetes-service)

Set the minimum node count to a value between 1 and 100 by using the `--min-count` parameter. A minimum node count is required when you enable the cluster autoscaler.

### Maximum Node Count
[Back to Top](#azure-kubernetes-service)

Set the maximum node count to a value between 1 and 100 by using the `--max-count` parameter. A maximum node count is required when you enable the cluster autoscaler.

## Deploy Pods to Spot Node Pools
[Back to Top](#azure-kubernetes-service)

When deploying workloads in Kubernetes, you can provide information to the scheduler to specify which nodes the workloads can or can't run. You control workload scheduling by configuring *taints*, *toleration*, or *node affinity*.

**Example**

```yml
apiVersion: v1
kind: Pod
metadata:
  name: nginx
  labels:
    env: test
spec:
  containers:
  - name: nginx
    image: nginx
    imagePullPolicy: IfNotPresent
  tolerations:
  - key: "kubernetes.azure.com/scalesetpriority"
    operator: "Equal"
    value: "spot"
    effect: "NoSchedule"
  affinity:
    nodeAffinity:
      requiredDuringSchedulingIgnoredDuringExecution:
        nodeSelectorTerms:
          - matchExpressions:
            - key: "kubernetes.azure.com/scalesetpriority"
              operator: In
              values:
              - "spot"
```

### What is a Taint?
[Back to Top](#azure-kubernetes-service)

A taint is applied to a node to indicate that only specific pods can be scheduled on it. Spot nodes are configured with a label set to `kubernetes.azure.com/scalesetpriority:spot`.

### What is Toleration?
[Back to Top](#azure-kubernetes-service)

Toleration is a specification applied to a pod to allow, but not require, a pod to be scheduled on a node with corresponding taint. Spot nodes are configured with a node taint set to `kubernetes.azure.com/scalesetpriority=spot:NoSchedule`.

### What is Node Affinity?
[Back to Top](#azure-kubernetes-service)

You use node affinity to describe which pods are scheduled on a node. Affinity is specified by using labels defined on the node. For example, in AKS, system pods are configured wiht anti-affinity towards spot nodes to prevent the pods from being scheduled on these nodes.

### Define Toleration in a Pod Manifest File
[Back to Top](#azure-kubernetes-service)

You specify node taint toleration by creating a `tolerations` dictionary entry in your workload manifest file. In this dictionary, you set the following properties for each node taint the workload has to tolerate in this section:

Property | Description
---------|------------
`key` | Identifies a node taint key-value pair specified on the node. For example, on a spot node pool, the key-value pair is `kubernetes.azure.com/scalesetpriority:spot`. The key is `kubernetes.azure.com/scalesetpriority`.
`operator` | Allows the toleration to match a taint. The default operator is `Equal`. You can also specify `Exists` to match toleration. However, when you use `Exists`, you don't specify the following property (`value`).
`value` | Represents the value part of the node taint key-value pair that is specified on the node. For example, on a spot node pool with a key-value pair of `kubernetes.azure.com/scalesetpriority:spot`, the value is `spot`.
`effect` | Indicates how the scheduling of a pod is handled in the system. There are three options: `NoSchedule`, `PreferNoSchedule`, and `NoExecute`. `NoSchedule` ensures that the system won't schedule the pod. `PreferNoSchedule` allows the system to try not to schedule the pod. `NoExecute` either evicts pods that are already running on the tainted node or doesn't schedule the pod at all.

### Define Node Affinity in a Pod Manifest File
[Back to Top](#azure-kubernetes-service)

You specify affinity by creating an `affinity` entry in your workload manifest file. In this entry, you set the following properties for each node label that a workload must match:

Property | Description
---------|------------
`nodeAffinity` | Describes node affinity scheduling rules for the pod.
`requiredDuringSchedulingIgnoredDuringExecution` | If the affinity requirements specified by this field aren't met at scheduling time, the pod won't be scheudled on the node. If the affinity requirements specified by this field cease to be met at some point during pod execution (for example, due ot an update), the system may or may not try to eventually evict the pod from its node.
`nodeSelectorTerms` | A list of node selector terms. The terms are ORed rather than ANDed.
`matchExpressions` | A list of node selector requirements by node's labels.
`key` | The label key that the selector applies to. The key is `kubernetes.azure.com/scalesetpriority`.
`operator` | Represents a key's relationship to a set of values. Valid operators are `In`, `NotIn`, `Exists`, `DoesNotExist`, `Gt` and `Lt`.
`values` | Represents the value part of the node label key-value pair that is specified on the node. On a spot node pool with a key-value pair of `kubernetes.azure.com/scaelsetpriority:spot`, the value is spot.

## Configure AKS Resource Quota Policies by using Azure Policy for Kubernetes
[Back to Top](#azure-kubernetes-service)

Azure Policy helps you to enforce standards and assess compliance at scale for your cloud environment.  It's good practice for companies to implement business rules to define how employees are to use company software, hardeware, and other resources in the organization. These business rules are often described by using policies that are put in place, enforced, and reviewwed as defined within each policy. A policy helps an organizatino meet governance and legal requiremetns, implement best practices, and establish organizational conventions.

Azure Kubernetes Service (AKS) enables you to orchestrate your cloud-native applications efficiently. You realize that you need to enforce business rules to manage how the teams use AKS to ensure a cost-effective approach to creating workloads. You can use Azure Policy to apply this same idea to how your Azure-based cloud resources are used.

### Kubernetes Admission Controller
[Back to Top](#azure-kubernetes-service)

[Reference](https://kubernetes.io/docs/reference/access-authn-authz/admission-controllers/)  

A Kubernetes plug-in that intercepts authenticated and authorized requests to the Kubernetes API before the requested Kubernetes object's persistence. You can think of an admission controller as software that governs and enforces how the cluster is used and designed. It limits requests to create, delete, and modify Kubernetes objects.

### Admission Controller Webhook
[Back to Top](#azure-kubernetes-service)

[Reference](https://kubernetes.io/docs/reference/access-authn-authz/extensible-admission-controllers/)

An HTTP callback function that receives admission requests and then acts on these requests. Admission controllers exist either as a compiled-in admission plug-in or as a deployed extension that runs as a webhook that you configure at runtime.

Admission webhooks are available in two kinds: either a *validating webhook* or a *mutating webhook*. A mutating webhook is invoked first and can change and apply defaults on the objects sent to the API server. A validation webhook validates object values and can reject requests.

### Open Policy Agent (OPA)
[Back to Top](#azure-kubernetes-service)

[Reference](https://www.openpolicyagent.org/docs/latest/)

An open-source, general-purpose policy engine that gives you a high-level declarative language to author policies. These policies enable you to define rules that oversee how your system should behave.

### OPA Gatekeeper
[Back to Top](#azure-kubernetes-service)

[Reference](https://www.openpolicyagent.org/docs/latest/kubernetes-introduction/#what-is-opa-gatekeeper)

An open-source, validating, Kubernetes admission-controller webhook that enforces Custom Resource Definition (CRD)-based policies by using the Open Policy Agent.

The goal of the OPA Gatekeeper is to define organization-wide policies. For example, you can require that:

* The maximum resource limits, such as CPU and memory limits, are enforced for all configured pods.
* The deployment of images is allowed only from approved repositories.
* Labels for all namespaces in a cluster specify a point of contact for each namespace.
* Cluster services have globally unique selectors.

The current veresion of the OPA Gatekeeper (version 3) is suppoerted by Azure Kubernetes Service.

### Azure Policy for AKS
[Back to Top](#azure-kubernetes-service)

Azure Policy extends OPA Gatekeeper version 3 and integrates with AKS through built-in policies. These policies apply at-scale enforcements and safeguards on your cluster in a centralized and consistent manner.

To setup resoure limits, you can apply resource quotas at the namespace level and monitor resource usage to adjust policy quotas. Use this strategy to reserve and limit resources across the development team.

### Assign a Built-In Policy Definition
[Back to Top](#azure-kubernetes-service)

You manage your Azure environment's policies by using the Azure policy compliance dashboard. The dashboard enables you to drill down to a per-resoruce, per-policy level of detail. It helps you bring your resources to compliance by using bulk remediation for existing resources and automatic remediation for new resources.

For each policy, the following overview information is listed:

Item | Description
-----|------------
Name | The name of the policy. For example, **[Preview]: Ensure container CPU and memory resoruce limits do not exceed the specified limits in Kubernetes cluster.**.
Scope | The subscription resource group to which this policy applies. For example, "Visual Studio Enterprise/rg-akscostsaving".
Compliance state | The status of assigned policies. The value can be **Compliant**, **Conflicted**, **Not Started**, or **Not Registered**.
Resource compliance | The percentage of resources that comply with the policy. This calculation takes into account compliant, non-compliant, and conflicting resources.
Non-compliant resources | The number of unique resources that violate one or more policy rules.
Non-compliant policies | The number of non-compliant policies.

From here, you drill down into the per-resource and per-policy details and events triggered.

## Assigning Policies
[Back to Top](#azure-kubernetes-service)

Azure Policies are assigned. To assign a policy, you select the **Assignments** option under the **Authoring** section in the Azure Policy navigatino panel.

You assign Azure policies in one of two ways: as a group of policies, called an *initiative*, or as a single policy.

## Initiative Assignment
[Back to Top](#azure-kubernetes-service)

A collection of Azure policy definitions grouped together to satisfy a specific goal or purpose.

## Policy Assignment
[Back to Top](#azure-kubernetes-service)

A policy assignment assigns a single policy, such as **Do not allow privileged containers in Kubernetes cluster**.

## How to Assign a Policy
[Back to Top](#azure-kubernetes-service)

Each policy is defined by using a series of configuration steps. The amount of information you capture depends on the type of policy you select.

### Basic Policy Information
[Back to Top](#azure-kubernetes-service)

The first step requires you to select and enter basic information that defines the new policy. This table shows each item you'll configure:

Item | Description
-----|------------
Scope | The scope determines what resource, or group of resources, the policy assignment is to be enforced on. This value is based on a subscription or a management group. You can exclude resources from your selection at one level lower than the scope level.
Policy Definition | The policy you want to apply. You can choose from several built-in poilcy options.
Assignment Name | The name by which to identify the assigned policy.
Description | A free-text description that describes the policy.
Policy enforcement | This option switches between **Enabled** and **Disabled**. If the option is **Disabled**, the policy isn't applied and requests aren't denied with non-compliance.
Assigned by | A free-text value that defaults to the restered user. This value can be changed.

### Policy Parameters

Policies require you to configure the business rules that apply to each specific policy. Not all policies have the same business rules, and that's why each policy has different parameters.

All policies have an **Effect** setting. This setting enables or disables the execution of the policy. As with parameters, policies can have different **Effect** options.

This table lists all the effects currently supported in policy definitions:

Effect | Description
-------|------------
Append | Adds more fields to the requested resource.
Audit | Creates a warning event in the activity log.
AuditIfNotExists | Enables auditing of resources related to the resource that matches the condition.
Deny | Prevents a resource request that doesn't match defined standards through a policy definition, and fails the request.
DeployIfNotExists | executes a template deployment when the condition is met.
Disabled | Useful for testing situations or for when the policy definition has parameterized the effect, and you want to disable a single assignment.
Modify | Adds, updates, or removes tags on a resource during creation or update.

### Policy Remediation

When you assign policies, it's possible that resources already exist and are impacted by the new policy. By default, only newly created resources are affected by the new policy. You can update existing resources by using a remediation task after you assign the policy. Remediation tasks differ depending on the types of policies applied.