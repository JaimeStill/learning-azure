# Optimize Compute Costs on Azure Kubernetes Service

* [Node Pools](#node-pools)
    * [System Node Pools](#system-node-pools)
    * [User Node Pools](#user-node-pools)
    * [Number of Nodes in a Node Pool](#number-of-nodes-in-a-node-pool)
* [Manage Application Demaind in an AKS Cluster](#manage-application-demand-in-an-aks-cluster)
    * [Scaling a Node Pool Manually](#scaling-a-node-pool-manually)
    * [Scaling a Cluster Automatically](#scaling-a-cluster-automatically)
        * [Horizontal Pod Autoscaler](#horizontal-pod-autoscaler)
        * [Cluster Autoscaler](#cluster-autoscaler)

## Node Pools
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

A node pool describes a group of nodes with the same configuration in an AKS cluster. These nodes contain the underlying VMs that run your applications. You can create two types of node pools on an AKS-managed Kubernetes cluster:

* System node pools
* User node pools

### System Node Pools
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

System node pools host critical system pods that make up the control plane of your cluster. A system node pool allows the use of Linux only as the node OS and runs only Linux-based workloads. Nodes in a system node pool are reserved for system workloads and normally not used to run custom workloads. Every AKS cluster must contain at least one system node pool with at least one node, and you must define the underlying VM sizes for nodes.

### User Node Pools
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

User node pools support your workloads, and you can specify Windows or Linux as the node operating system. You can also define the underlying VM sizes for nodes and run specific workloads. For example, a solution has a batch-processing service that you deploy to a node pool configured with general-purpose VMs. The new predictive-modeling service requires higher-capacity, GPU-based VMs. You decide to configure a separate node pool and configure it to use GPU-enabled nodes.

### Number of Nodes in a Node Pool
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

You can configure up to 100 nodes in a node pool. However, the number of nodes you choose to configure depends on the number of pods that run per node.

For example, in a system node pool, it's essential to set the maximum number of pods that run on a single node to 30. This value guarantees that enough space is available to run teh system pods critical to cluster health. When the number of pods exceeds this minimum value, new nodes are required in teh pool to schedule additional workloads. For this reason, a system node pool needs at least one node in the pool. For production environments, the recommended node count for a system node pool is a minimum of three nodes.

User node pools are designed to run custom workloads and don't have the 30-pod requirement. User node pools allow you to set the node count for a pool to zero.

## Manage Application Demand in an AKS Cluster
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

The function in AKS that provides for increasing or decreasing the amount of compute resources in a Kubernetes cluster is called *scaling*. You scale either the number of workload instances that need to run or the number of nodes on which these workloads run. You scale workloads on an AKS-managed cluster in one of two ways. The first option is to scale the pods or nodes manually as necessary. Or, you can use the horizontal pod autoscaler to scale pods and the cluster autoscaler to scale the nodes.

### Scaling a Node Pool Manually
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

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
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

![cluster-autoscaler](https://learn.microsoft.com/en-us/training/modules/aks-optimize-compute-costs/media/2-cluster-autoscaler.png)

AKS uses the Kubernetes cluster autoscaler to automatically scale workloads. The cluster can scale by using two options:

* The horizontal pod autoscaler
* The cluster autoscaler

#### Horizontal Pod Autoscaler
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Use the Kubernetes horizontal pod autoscaler to monitor the resource demand on a cluster and automatically scale the number of workload replicas.

The Kubernetes Metrics Server collects memory and processor metrics from controllers, nodes, and containers that run on the AKS cluster. One way to access this information is to use the Metrics API. The horizontal pod autoscaler checks the Metrics API every 30 seconds to decide whether your application needs additional instances to meet the required demand.

Assume your company also has a batch-processing service that schedules drone flight paths. You see the service gets inundated with requests and builds up a backlog of deliveries, causing delays and frustrations for customers. Increasing the number of batch-processing service replicas will enable the timely processing of orders.

To solve the problem, you configure the horizontal pod autoscaler to scale up the number of service replicas when needed. When batch requests decrease, it scales the replica count down.

However, the horizontal pod autoscaler scales pods only on available nodes in the configured node pools of the cluster.

#### Cluster Autoscaler
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

A resource constraint is triggered when the horizontal pod autoscaler can't schedule additional pods on the existing nodes in a node pool. You use the cluster autoscaler to scale the number of nodes in a cluster's node pools. The cluster autoscaler checks the defined metrics and scales the number of nodes up or down based on the computing resources required.

The cluster autoscaler is used alongside the horizontal pod autoscaler.

The cluster autoscaler monitors for both scale-up and scale-down events, and allows the Kubernetes cluster to change the node count ina node pool as resource demands change.

You configure each node pool with different scale rules. For example, you might want to configure only one node pool to allow autoscaling. Or you might configure a node pool to scale only to a specific number of nodes.