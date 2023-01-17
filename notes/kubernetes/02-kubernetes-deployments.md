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

### Grouping Pods
[Back to Top](#kubernetes-deployments)



### Storage
[Back to Top](#kubernetes-deployments)



## Cloud Integration
[Back to Top](#kubernetes-deployments)

