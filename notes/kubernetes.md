# Kubernetes

## Architecture
[Back to Top](#kubernetes)

Kubernetes is used as the orchestration and cluster software to deploy apps and respond to changes in compute resource needs.

### Orchestrator
[Back to Top](#kubernetes)

A system that deploys and manages apps.

### Cluster
[Back to Top](#kubernetes)

A cluster is a set of computers that you configure to work together and view as a single system. They computers configured in the cluster will typically do the same kinds of tasks (i.e. - host websites, APIs, or run compute-intensive work).

A cluster uses centralized software that's responsible for scheduling and controlling these tasks. The computers in a cluster that run the tasks are called *nodes*, and the computers that run the scheduling software are called *control planes*.

![cluster-diagram](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/3-diagram-cluster.svg)

![cluster-components](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/3-cluster-architecture-components.svg)

A Kubernetes cluster contains at least one main plane and one or more nodes. Both the control planes and node instances can be physical devices, virtual machines, or instances in the cloud. The default host OS in Kubernetes is Linux, with default support for Linux-based workloads.

### Control Plane
[Back to Top](#kubernetes)

The control plane in a cluster runs a collection of services that manage the orchestration functionality in Kubernetes. In production and cloud deployments, the preferred configuration is a high-availability deployment with three to five replicated control planes.

Kubernetes relies on several administrative services running on the control plane. These services manage aspects, such as cluster component communication, workload scheduling, and cluster state persistence.

The following services make up the control plane for a Kubernetes cluster:

* API server
* Backing store
* Scheduler
* Controller manager
* Cloud controller manager


#### API Server
[Back to Top](#kubernetes)

The front end to the control plane in your Kubernetes cluster. All communication between the components in Kubernetes is done through this API.

The `kubectl` command-line app enables you to run commands against the Kubernetes cluster's API server. The component that provides this API is called `kube-apiserver`, and you can deploy several instances of this component to support scaling in your cluster.

#### Backing Store
[Back to Top](#kubernetes)

A persistence store that your Kubernetes cluster uses to save the complete configuration of a Kubernetes cluster. Kubernetes uses a high-availability, distributed, and reliable key-value store called `etcd`. This key-value store stores the current state and the desired state of all objects within your cluster.

In a production Kubernetes cluster, the official Kubernetes guidance is to have three to five replicated instances of the `etcd` database for high availability.

#### Scheduler
[Back to Top](#kubernetes)

The component that's responsible for the assignment of workloads across all nodes. The scheduler monitors the cluster for newly created containers, and assigns them to nodes.

#### Controller Manager
[Back to Top](#kubernetes)

Launches and monitors the controllers configured for a cluster through the API server.

Kubernetes uses controllers to trakc the state of objects in the cluster. Each controller runs in a non-terminating loop while watching and responding to events in the cluster. For example, there are controllers to monitor nodes, containers, and endpoints.

The controller communicates with the API server to determine the state of the object. If the current state is different from the watned state of the object, the controller will take action to ensure the wanted state.

Assume that one of three containers running in your cluster stops responding and has died. In this case, a controller decides whether you need to launch new containers to ensure that your apps are always available. If the desired state is to run three containers at a time, then a new container is scheduled to run.

#### Cloud Controller Manager
[Back to Top](#kubernetes)

Integrates with the underlying cloud technologies in your cluster when the cluster is running in a cloud environment. These services can be load balancers, queues, and storage, for example.

### Node
[Back to Top](#kubernetes)

A node in a cluster is where your compute workloads run. Each node communicates with the control plane via the API server to inform it about state changes on the node.

The following services run on the Kubernetes node:
* Kubelet
* Kube-proxy
* Container runtime

#### kubelet
[Back to Top](#kubernetes)

The agent that runs on each node in the cluster, and monitors work requests from the API server. It makes sure the requested unit of work is running and healthy.

The kubelet monitors the nodes and makes sure that the containers scheduled on each node run as expected. The kubelet manages only containers created by Kubernetes. It isn't responsible for rescheduling work to run on other nodes if the current node can't run the work.

#### kube-proxy
[Back to Top](#kubernetes)

Responsible for local cluster networking, and runs on each node. It ensures that each node has a unique IP address. It also implements rules to handle routing and load balancing of traffic by using iptables and IPVS.

This proxy doesn't provide DNS services by itself. A DNS cluster add-on based on CoreDNS is recommended and installed by default.

#### Container Runtime
[Back to Top](#kubernetes)

The underlying software that runs containers on a Kubernetes cluster. The runtime is responsible for fetching, starting, and stopping container images. Kubernetes supports several container runtims, including but not limited to Docker, containerd, rkt, CRI-O, and frakti. The support for many container runtime types is based on the Container Runtime Interface (CRI). The CIR is a plug-in design that enables a kubelet to communicate with the available container runtime.

The default container runtim in AKS is Docker. However, AKS now supports creation of clusters and node pools with containerd, an industry-standard container runtime, in preview. Containerd will become the default container runtime for AKS when containerd support becomes generally available.

### kubectl
[Back to Top](#kubernetes)

A command-line tool to manage your cluster. You use `kubectl` to send commands to the cluster's control plane, or fetch informatino about all Kubernetes objects via the API server.

`kubectl` uses a configuration file that includes the following configuration information:
* **Cluster** configuration specifies a cluster name, certificate information, and the service API endpoint associated with the cluster. This definition enables you to connect from a single workstation to multiple clusters.
* **User** configuration specifies the users and their permission levels when they're accessing the configured clusters.
* **Context** configuration groups clusters and users by using a friendly name. For example, you might have a "dev-cluster" and a "prod-cluster" to identify your development and production clusters.

You can configure `kubectl` to connect to multiple clusters by providing the correct context as part of the command-line syntax.

### Pods
[Back to Top](#kubernetes)

Represents a single instance of an app running in Kubernetes. THe workloads that you run on Kubernetes are containerized apps. Unlike in a Docker environment, you can't run containers directly on Kubernetes. You package the container into a Kubernetes object called a pod. A pod is the smallest object that you can create in Kubernetes.

![website-pod](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/3-diagram-pod-with-website.svg)

A single pod can hold a group of one or more containers. However, a pod typically doesn't contain multiples of the same app.

A pod includes information about the shared storage and network configuration, and a specification about how to run its packaged containers. You use pod templates to define the information about the pods that run in your cluster. Pod templates are YAML-coded files that you reuse and include in other objects to manage pod deployments.

![website-and-database-pod](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/3-diagram-pod-with-website-database.svg)

Assume you want to deploy a website to a Kubernetes cluster. You create the pod definition file that specifies the app's container images and configuration. Next, you deploy the pod definition file to Kubernetes.

It's unlikely that a web app has a website as the only component in the solution. A web app typically has some kind of datastore and other supporting elements. Kubernetes pods can also contain more than one container.

Assume that your site uses a database. The website is packaged in the main container, and the database is packaged in the supporting container. For these two containers to functino and communicate with each other, you expect them to run in an environment that provides a host OS, a network stack, kernel namespaces, shared memory, and volumes to persist data. The pod is the sandbox environment that provides all of these services to your app. The pod also allows the containers to share its assigned IP address.

Because you can potentially create many pods that are running on many nodes, it can be hard to identify them. You recognize and group pods by using string labels that you specify when you define a pod.

#### Lifecycle
[Back to Top](#kubernetes)

Kubernetes pods have a distinct lifecycle that affects the way you deploy, run, and update pods. You start by submitting the pod YAML manifest to the cluster. After the manifest file is submitted and persisted to the cluster, it defines the desired state of the pod. The scheduler schedules the pod to a healthy node that has enough resources to run the pod.

![pod-lifecycle](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/3-pod-lifecycle.svg)

Phase | Description
------|------------
`Pending` | The pod has been accepted by the cluster, but one or more containers isn't setup or ready to run. The `Pending` status indicates the time a pod is waiting to be scheduled and the time spent downloading container images.
`Running` | The pod transitions to a running state after all of the resources within the pod are ready.
`Succeeded` | The pod transitions to a succeeded state after the pod completes its intended task, and runs successfully.
`Failed` | Pods can fail for various reasons. A container int he pod may have failed, leading to the termination of all other containers. Or, maybe an image wasn't found during preparation of the pod containers. In these types of cases, the pod can go to a failed state. Pods can transition to a failed state from either a pending state or a running state. A specific failure can also place a pod back in a pending state.
`Unknown` | If the state of the pod can't be determined, the pod is in an unknown state.

Pods are kept on a cluster until a controller, the control plane, or a user explicitly removes them. When a pod is deleted and is replaced by a new pod, the new pod is an entirely new instance of the pod based on the pod manifest.

The cluster doesn't save the pod's state or dynamically assigned configuration. For example, it doesn't save the pod's ID or IP address. This aspect affects how you deploy pods and how you design your apps. For example, you can't rely on preassigned IP addresses for your pods.

#### Container States
[Back to Top](#kubernetes)

Keep in mind that the phases are a summary of where the pod is in its lifecycle. When you inspect a pod, the cluster uses three states to track your containers inside the pod:

State | Description
------|------------
`Waiting` | Default state of a container and the state that the container is in when it's not running or terminated.
`Running` | The container is running as expected without any problems.
`Terminated` | The container is no longer running. The reason is that either all tasks finished, or the container failed for some reason. A reason and exit code are available for debugging both cases.

## Deployments

### Pod Deployment Options
[Back to Top](#kubernetes)

There are several options to manage the deployment of pods in a Kubernetes cluster when you're using `kubectl`. The options are:

* Pod templates
* Replication controllers
* Replica sets
* Deployments

You can use any of these four Kubernetes object type definitions to deploy a pod or pods. These files make use of YAML to describe the intended state of the pod or pods that will be deployed.

#### Pod Template
[Back to Top](#kubernetes)

Enables you to define the configuration of the pod you want to deploy. The template contains information, such as the name of the image, and which container registry to use to fetch the images. The template may also include runtime configuration information, such as ports to use. Templates are defined by using YAML in the same way s when you create Docker files.

You can use templates to deploy pods manually. However, a manually-deployed pod isn't relaunched after it fails, is deleted, or is terminated. To manage the lifecycle of a pod, you need to create a higher-level Kubernetes object.

#### Replication Controller
[Back to Top](#kubernetes)

Uses pod templates and defines a specified number of pods that must run. The controller helps you run multiple instances of the same pod, and ensures pods are always running on one or more nodes in the cluster. The controller replaces running pods in this way with new pods if they fail, are deleted, or are terminated.

#### Replica Set
[Back to Top](#kubernetes)

Replaces the replication controller as the preferred way to dpeloy replicas. A replica set includes the same functionality as a replication controller. However, it has an extra configuration option to include a selector value.

A selector enables the replica set to identify all the pods running underneath it. Using this feature, you can manage pods labeled with the same value as the selector value, but not created with the replicated set.

#### Deployment
[Back to Top](#kubernetes)

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

### Deployment Considerations
[Back to Top](#kubernetes)

Kubernetes has specific requirements about how you configure networking and storage for a cluster. How you configure these two aspects affects your decisions about how to expose your apps on the cluster network and store data.

#### Networking
[Back to Top](#kubernetes)

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

#### Services
[Back to Top](#kubernetes)

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

#### Grouping Pods
[Back to Top](#kubernetes)

Managing pods by IP address isn't practical. Pod IP addresses change as controllers re-create them, and you might have any number of pods running.

![service-with-selector](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/4-service-with-selector.svg)

A service object enables you to target and manage specific pods in your cluster by using selector labels. You set the selector label in a service definition to match the pod label defined in the pod's definition file.

Assume that you have many running pods. Only a few of these pods are on the front end, and you want to set a LoadBalancer service that targets only the front-end pods. You can apply your service to expose these pods by referencing the pod label as a selector value in the service's definition file. The service will now group only the pods that match the label. If a pod is removed and re-created, the new pod is automatically added to the service group through its matching label.

#### Storage
[Back to Top](#kubernetes)

Kubernetes uses the same storage volume concept that you find when using Docker. Docker volumes are less managed than the Kubernetes volumes because Docker volume lifetimes aren't managed. The Kubernetes volume's lifetime is an explicit lifetime that matches the pod's lifetime. The lifetime match means a volume outlives the containers that run in the pod. However, fi the pod is removed, so is the volume.

Kubernetes provides options to provision persistent storage with the use of *Persistent Volumes*. You can also request specific storage for pods by using *PersistenVolumeClaims*.

Keep both of these options in mind when you're deploying app components that require persisted storage, like message queues and databases.

### Cloud Integration
[Back to Top](#kubernetes)

Kubernetes doesn't dictate the technology stack that you use in your cloud-native app. In a cloud environment such as Azure, you can use several services outside the Kubernetes cluster.

Recall from earlier that Kubernetes doesn't provide any of the following services:

* Middleware
* Data-processing frameworks
* Databases
* Caches
* Cluster storage systems

In the scenario described in the [Services](#services) section, there are three services that provide middleware functionality - a NoSQL database, an in-memory cache service, and a message queue. You might select MongoDB Atlas for the NoSQL solution, Redis to manage the in-memory cache and RabbitMQ, or Kafka, depending on you rmessage queue needs.

When you're using a cloud environment such as Azure, it's a best practice to use services outside the Kubernetes cluster. This decision can simplify the cluster's configuration and management. For example, you can use *Azure Cache for Redis* for the in-memory caching services, *Azure Service Bus messaging* for the message queue, and *Azure Cosmos DB* for the NoSQL database.

## Cheatsheet
[Back to Top](#kubernetes)

```bash
# create a deployment file
touch deploy.yaml
```

> [deploy.yaml](./assets/kubernetes/deploy.yaml)

```bash
# Deploy based on deploy.yaml
kubectl apply -f deploy.yaml

# test deployed service
# after starting, http://[external-ip-address]/WeatherForecast
kubectl get service mymicroservice --watch

# scale to two instances
kubectl scale --replicas=2 deployment/mymicroservice

# get all running containers
kubectl get pods

# delete container instance
kubectl delete pod {name}
```