# Kubernetes Deployments

* [Pod Deployment Options](#pod-deployment-options)
    * [Pod Template](#pod-template)
    * [Replication Controller](#replication-controller)
    * [Replica Set](#replica-set)
    * [Deployment](#deployment)
* [Deployment Considerations](#deployment-considerations)
    * [Networking](#networking)
    * [Services](#services)
    * [Grouping Pods](#grouping-pods)
    * [Storage](#storage)
* [Cloud Integration](#cloud-integration)

## Pod Deployment Options
[Back to Top](#kubernetes-deployments)

There are several options to manage the deployment of pods in a Kubernetes cluster when you're using `kubectl`. The options are:

* Pod templates
* Replication controllers
* Replica sets
* Deployments

You can use any of these four Kubernetes object type definitions to deploy a pod or pods. These files make use of YAML to describe the intended state of the pod or pods that will be deployed.

### Pod Template
[Back to Top](#kubernetes-deployments)

Enables you to define the configuration of the pod you want to deploy. The template contains information, such as the name of the image, and which container registry to use to fetch the images. The template may also include runtime configuration information, such as ports to use. Templates are defined by using YAML in the same way s when you create Docker files.

You can use templates to deploy pods manually. However, a manually-deployed pod isn't relaunched after it fails, is deleted, or is terminated. To manage the lifecycle of a pod, you need to create a higher-level Kubernetes object.

### Replication Controller
[Back to Top](#kubernetes-deployments)

Uses pod templates and defines a specified number of pods that must run. The controller helps you run multiple instances of the same pod, and ensures pods are always running on one or more nodes in the cluster. The controller replaces running pods in this way with new pods if they fail, are deleted, or are terminated.

### Replica Set
[Back to Top](#kubernetes-deployments)

Replaces the replication controller as the preferred way to dpeloy replicas. A replica set includes the same functionality as a replication controller. However, it has an extra configuration option to include a selector value.

A selector enables the replica set to identify all the pods running underneath it. Using this feature, you can manage pods labeled with the same value as the selector value, but not created with the replicated set.

### Deployment
[Back to Top](#kubernetes-deployments)

Creates a management object one level higher than a replica set, and enables you to deploy and manage updates for pods in a cluster.

Assume you have five instances of your app deployed in your cluster. There are five pods running version 1.0.0 of your app.

![pods-running-same-version](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/4-pods-running-same-version.svg)

If you decide to update your app manually, you can remove all pods, and then launch new pods running version 2.0.0 of your app. With this strategy, your app will experience downtime.

Instead, you'll want to execute a rolling update, whereby you launch pods with the new version of your app before you remove the older app versioned pods. Rolling updates will launch one pod at a time instead of taking down all the older pods at once. Deployments honor the number of replicas configured in the section that describes information about replica sets. It will maintain the number of pods specified in the replica set as it replaces old pods with new pods.

![pods-running-different-version](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/4-pods-running-different-version.svg)

Deployments, by default, provide a rolling update strategy for updating pods. You can also use a re-create strategy. This strategy will terminate pods before launching new pods.

Deployments also provide you with a rollback strategy, which you can execute by using `kubectl`.

Deployments make use of YAML-based definition files, and make it easy to manage deployments. Keep in mind that deployments enable you to apply any changes to your cluster. For example, you can deploy new versions of an app, update labels, and run other replicas of your pods.

`kubectl` has convenient syntax to create a deployment automatically when you're using the `kubectl run` command to deploy a pod. This command creates a deployment with the required replica set and pods. However, the command doesn't create a definition file. A best practice is to manage all deployments with deployment definition files, and track changes by using a version control system.

## Deployment Considerations
[Back to Top](#kubernetes-deployments)

Kubernetes has specific requirements about how you configure networking and storage for a cluster. How you configure these two aspects affects your decisions about how to expose your apps on the cluster network and store data.

### Networking
[Back to Top](#kubernetes-deployments)

Assume you have a cluster with one control plane and two nodes. When you add nodes to Kubernetes, an IP address is automatically assigned to each node from an internal private network range. For example, assume that your local network range is 192.168.1.0/24

![nodes-assigned-ip-addresses](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/4-nodes-assigned-ip-addresses.svg)

Each pod that you dpeloy gets assigned an IP from a pool of IP addresses. For example, assume that your configuration uses the 10.32.0.0/12 network range, as the following image shows.

![nodes-pods-assigned-ip-addresses](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/4-nodes-pods-assigned-ip-addresses.svg)

By default, the pods and nodes can't communicate with each other by using different IP address ranges.

To further complicate matters, recall that pods are transient. The pod's IP address is temporary, and can't be used to reconnect to a newly created pod. This configuration affects how your app communicates with its internal components, and how you and services interact with it externally.

To simplify communication, Kubernetes expects you to configure networking in such a way that:

* Pods can commuinicate with one another across nodes without Network Address Translation (NAT)
* Nodes can communicate with all pods, and vice versa, without NAT
* Agents on a node can communicate with all nodes and pods

Kubernetes offers several networking options that you can install to configure networking. Examples include Antrea, Cisco Application Centric Infrastructure (ACI), Cilium, Flannel, Kubenet, VMware NSX-T, and Weave Net.

Cloud providers also provide their own networking solutions. For example, Azure Kubernetes Service (AKS) supports the Azure Virtual Network container network interface (CNI), Kubenet, Flannet, Cilium, and Antrea.

### Services
[Back to Top](#kubernetes-deployments)

A Kubernetes service is a Kubernetes object that provides stable networking for pods. A Kubernetes service enables communication between nodes, pods, and users of your app, both internal and external, to the cluster.

Kubernetes assigns a service an IP address on creation, just like a node or pod. These addresses get assigned from a service cluster's IP range. An example is 10.96.0.0/12. A service is also assigned a DNS name based on the service name, and an IP port.

Consider a scenario where an app has network communication as follows:

* The website and RESTful API are accessible to users outside the cluster
* The in-memory cache and message queue services are accessible to the front end and the RESTful API, respectively, but not to external users
* The message queue needs access to the data processing service, but not to external users
* The NoSQL database is accessible to the in-memory cache and data processing service, but not to external users

To support these scenarios, you can configure three types of services to expose your app's components.

Service | Description
--------|------------
`ClusterIP` | The address assigned to a service that makes the service available to a set of services inside the cluster. For example, communication between the front-end and back-end components of your app.
`NodePort` | The node port, between 30000 and 32767, that the Kubernetes control plane assigns to the service. An example is 192.158.1.11 on clusters01. You then configure the service with a target port on the pod that you want to expose. For example, configure port 80 on the pod running one of the fornt ends. YOu can now access the front end through a node IP and port address.
`LoadBalancer` | The load balancer that allows for the distribution of load between nodes running your app, and exposing the pod to public network access. You typically configure load balancers when you use cloud providers. In this case, traffic from the external load balancer is directed to the pods running your app.

In the above scenario, you might decide to expose the website and the RESTful API by using a LoadBalancer and the data processing service by using a ClusterIP.

### Grouping Pods
[Back to Top](#kubernetes-deployments)

Managing pods by IP address isn't practical. Pod IP addresses change as controllers re-create them, and you might have any number of pods running.

![service-with-selector](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/4-service-with-selector.svg)

A service object enables you to target and manage specific pods in your cluster by using selector labels. You set the selector label in a service definition to match the pod label defined in the pod's definition file.

Assume that you have many running pods. Only a few of these pods are on the front end, and you want to set a LoadBalancer service that targets only the front-end pods. You can apply your service to expose these pods by referencing the pod label as a selector value in the service's definition file. The service will now group only the pods that match the label. If a pod is removed and re-created, the new pod is automatically added to the service group through its matching label.

### Storage
[Back to Top](#kubernetes-deployments)

Kubernetes uses the same storage volume concept that you find when using Docker. Docker volumes are less managed than the Kubernetes volumes because Docker volume lifetimes aren't managed. The Kubernetes volume's lifetime is an explicit lifetime that matches the pod's lifetime. The lifetime match means a volume outlives the containers that run in the pod. However, fi the pod is removed, so is the volume.

Kubernetes provides options to provision persistent storage with the use of *Persistent Volumes*. You can also request specific storage for pods by using *PersistenVolumeClaims*.

Keep both of these options in mind when you're deploying app components that require persisted storage, like message queues and databases.

## Cloud Integration
[Back to Top](#kubernetes-deployments)

Kubernetes doesn't dictate the technology stack that you use in your cloud-native app. In a cloud environment such as Azure, you can use several services outside the Kubernetes cluster.

Recall from earlier that Kubernetes doesn't provide any of the following services:

* Middleware
* Data-processing frameworks
* Databases
* Caches
* Cluster storage systems

In the scenario described in the [Services](#services) section, there are three services that provide middleware functionality - a NoSQL database, an in-memory cache service, and a message queue. You might select MongoDB Atlas for the NoSQL solution, Redis to manage the in-memory cache and RabbitMQ, or Kafka, depending on you rmessage queue needs.

When you're using a cloud environment such as Azure, it's a best practice to use services outside the Kubernetes cluster. This decision can simplify the cluster's configuration and management. For example, you can use *Azure Cache for Redis* for the in-memory caching services, *Azure Service Bus messaging* for the message queue, and *Azure Cosmos DB* for the NoSQL database.
