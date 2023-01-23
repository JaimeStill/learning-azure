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
* [Configure Multiple Nodes and Enable Scale-to-Zero on an AKS Cluster](#configure-multiple-nodes-and-enable-scale-to-zero-on-an-aks-cluster)
    * [Create a New Resource Group](#create-a-new-resource-group)
    * [Create the AKS Cluster](#create-the-aks-cluster)
    * [Add a Node Pool](#add-a-node-pool)
    * [Scale the Node Pool Node Count to Zero](#scale-the-node-pool-node-count-to-zero)
    * [Configure the Kubernetes Context](#configure-the-kubernetes-context)
* [Spot VM](#spot-vm)
    * [Spot VM Availability](#spot-vm-availability)
    * [Spot VM Eviction Policy](#spot-vm-eviction-policy)
    * [Spot VM Scale Set](#spot-vm-scale-set)
* [Spot Node Pool](#spot-node-pool)
    * [Spot Pool Limitations](#spot-pool-limitations)
* [Add a Spot Node Pool to an AKS Cluster](#add-a-spot-node-pool-to-an-aks-cluster)
    * [Priority](#priority)
    * [Eviction Policy](#eviction-policy)
    * [Maximum Price for Spot Node](#maximum-price-for-spot-node)
    * [Enable the Cluster Autoscaler](#enable-the-cluster-autoscaler)
    * [Minimum Node Count](#minimum-node-count)
    * [Maximum Node Count](#maximum-node-count)
* [Deploy Pods to Spot Node Pools](#deploy-pods-to-spot-node-pools)
    * [What is a Taint?](#what-is-a-taint)
    * [What is Toleration?](#what-is-toleration)
    * [What is Node Affinity?](#what-is-node-affinity)
    * [Define Toleration in a Pod Manifest File](#define-toleration-in-a-pod-manifest-file)
    * [Define Node Affinity in a Pod Manifest File](#define-node-affinity-in-a-pod-manifest-file)

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

For example, in a system node pool, it's essential to set the maximum number of pods that run on a single node to 30. This value guarantees that enough space is available to run the system pods critical to cluster health. When the number of pods exceeds this minimum value, new nodes are required in the pool to schedule additional workloads. For this reason, a system node pool needs at least one node in the pool. For production environments, the recommended node count for a system node pool is a minimum of three nodes.

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

## Configure Multiple Nodes and Enable Scale-to-Zero on an AKS Cluster
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Here, 'youll create an Azure Kubernetes Service (AKS)-managed Kubernetes cluster. You'll configure the cluster to support multiple node pool and make sure the cluster allows you to scale the nodes in the node pools. Then you'll add a second node pool to support user workloads. Finally, you'll scale the node count in the second node pool to zero to reduce the cost of the node used in your AKS cluster.

### Create a New Resource Group
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

1. Sign into the Azure CLI:

    ```bash
    az login
    ```

2. You'll reuse some value throughout the exercises in this module. For example, you'll need to choose a region where you want to create a resource group. Some features that you'll add in later exercises might not be available in the region you select.

    Run the following to register variables:

    ```bash
    REGION_NAME=eastus
    RESOURCE_GROUP=rg-akscostsaving
    # akscostsaving-25841
    AKS_CLUSTER_NAME=akscostsaving-$RANDOM
    ```

    You can check each value by running the `echo` command, for example, `echo $AKS_CLUSTER_NAME`.

3. Make a note of your new cluster's name. You'll use this value later when you clean up resources, and in configuration settings for the cluster:

    ```bash
    echo $AKS_CLUSTER_NAME
    ```

4. Create a new resource group named **rg-akscostsaving**. Deploy all resources created in these exercises to this resource group. A single resource group makes it easier to clean up the resources after you finish the module:

    ```bash
    az group create \
        --name $RESOURCE_GROUP \
        --location $REGION_NAME
    ```

### Create the AKS Cluster
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

With the resource group in place, you can now create the AKS cluster. Your first step is to get the version of the latest, non-preview version of Kubernetes in your selected region. You'll use this version in configuring the cluster.

1. To get the latest, non-preview Kubernetes verseion, run the `az aks get-versions` command. Store the value that the command returns in a Bash variable named `VERSION`:

    **Saved as [`get-kubernetes-version.sh`](./assets/optimize-compute-costs/get-kubernetes-version.sh)**  

    ```bash
    # piping with tr -d '\r' needed for removing
    # carriage return when running in WSL
    VERSION=$(az aks get-versions \
        --location $REGION_NAME \
        --query 'orchestrators[?!ispreview] | [-1].orchestratorVersion' \
        --output tsv | tr -d '\r')
    ```

2. Run the `az aks create` command shown later in this step to create the AKS cluster. The cluster will run the latest Kubernetes version with two nodes in the system node pool. This command can take a few minutes to finish.

    The `az aks create` command has several parameters that enable precise configuration of your Kubernetes cluster. There are two important parameters in configuring the correct support in your cluster for scaling and multiple node pools:

    Parameter and value | Description
    --------------------|------------
    `--load-balancer-sku standard` | The default load-balancer support in AKS is `basic`. The `basic` load balancer isn't supported when you use multiple node pools. Set the value to `standard`.
    `--vm-set-type VirtualMachineScaleSets` | To use the scale features in AKS, virtual machine scale sets are required. This parameter enables support for scale sets.

    **Saved as [`create-aks.sh`](./assets/optimize-compute-costs/create-aks.sh)**  

    ```bash
    az aks create \
        --resource-group $RESOURCE_GROUP \
        --name $AKS_CLUSTER_NAME \
        --location $REGION_NAME \
        --kubernetes-version $VERSION \
        --node-count 2 \
        --load-balancer-sku standard \
        --vm-set-type VirtualMachineScalSets \
        --generate-ssh-keys
    ```

    Notice that two nodes are configured in the default node pool by using the `--node-count 2` parameter. Recall from previous description that essential system services run across this system node pool. It's important that production clusters use at least `--node-count 3` for reliability in cluster operation. We're using only two nodes here for cost considerations in this exercise.

3. Run the `az aks nodepool list` command to list the node pools in your new cluster:

    ```bash
    az ak nodepool list \
        --resource-group $RESOURCE_GROUP \
        --cluster-name $AKS_CLUSTER_NAME \
    ```

    Output:

    ```json
    [
        {
            "agentPoolType": "VirtualMachineScaleSets",
            "availabilityZones": null,
            "count": 2,
            "enableAutoScaling": null,
            "enableNodePublicIp": false,
            "id": "/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourcegroups/rg-akscostsaving/providers/Microsoft.ContainerService/managedClusters/akscostsaving-25841/agentPools/nodepool1",
            "mode": "System",
            "name": "nodepool1",
            ...
            "type": "Microsoft.ContainerService/managedClusters/agentPools",
            "upgradeSettings": null,
            "vmSize": "Standard_DS2_v2",
            "vnetSubnetId": null
        }
    ]
    ```

    Notice that the `mode` of the node pool is set to `System` and that the `name` is automatically assigned.

### Add a Node Pool
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

1. Your cluster has a single node pool. Add a second node pool by running the `az aks nodepool add` command. Run the command in this step to create a user node pool with thre nodes and the name `batchprocpl`. Keep in mind that node-pool names must start with a lowercase letter and contain only alphanumeric characters. Node-pool names are limited to 12 characters for Linux node pools and six characters for Windows node pools:

    **Saved as [`add-aks-nodepool.sh`](./assets/optimize-compute-costs/add-aks-nodepool.sh)**  

    ```bash
    az aks nodepool add \
        --resource-group $RESOURCE_GROUP \
        --cluster-name $AKS_CLUSTER_NAME \
        --name batchprocpl
        --node-count 2
    ```

2. Run the `az aks nodepool list` command to list the new node pool in your new cluster:

    ```bash
    az aks nodepool list --resource-group $RESOURCE_GROUP --cluster-name $AKS_CLUSTER_NAME
    ```

    Output:

    ```json
    [
        {
            "agentPoolType": "VirtualMachineScaleSets",
            "availabilityZones": null,
            "count": 2,
            "enableAutoScaling": null,
            "enableNodePublicIp": false,
            "id": "/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourcegroups/rg-akscostsaving/providers/Microsoft.ContainerService/managedClusters/akscostsaving-25841/agentPools/batchprocpl",
            "mode": "User",
            "name": "batchprocpl",
            ...
            "type": "Microsoft.ContainerService/managedClusters/agentPools",
            "upgradeSettings": {
            "maxSurge": null
            },
            "vmSize": "Standard_DS2_v2",
            "vnetSubnetId": null
        },
        {
            "agentPoolType": "VirtualMachineScaleSets",
            "availabilityZones": null,
            "count": 2,
            "enableAutoScaling": null,
            "enableNodePublicIp": false,
            "id": "/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourcegroups/rg-akscostsaving/providers/Microsoft.ContainerService/managedClusters/akscostsaving-25841/agentPools/nodepool1",
            "mode": "System",
            "name": "nodepool1",
            ...
            "type": "Microsoft.ContainerService/managedClusters/agentPools",
            "upgradeSettings": null,
            "vmSize": "Standard_DS2_v2",
            "vnetSubnetId": null
        }
    ]
    ```

    Notice that the `mode` of the new node pool is set to `User` and that the `name` is `batchprocpl`.

### Scale the Node Pool Node Count to Zero
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

You run the `az aks nodepool scale` command to scale nodes in a node pool manually.

Run the `az aks nodepool scale` command and use the `--node-count` parameter to set the node-count value to 0:

**Saved as [`scale-node-to-zero.sh`](./assets/optimize-compute-costs/scale-node-to-zero.sh)  

```bash
az aks nodepool scale \
    --resource-group $RESOURCE_GROUP \
    --cluster-name $AKS_CLUSTER_NAME \
    --name batchprocpl \
    --node-count 0
```

Output:

```json
{
  "agentPoolType": "VirtualMachineScaleSets",
  "availabilityZones": null,
  "count": 0,
  "enableAutoScaling": null,
  "enableNodePublicIp": false,
  "id": "/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourcegroups/rg-akscostsaving/providers/Microsoft.ContainerService/managedClusters/akscostsaving-17835/agentPools/batchprocpl",
  "maxCount": null,
  "maxPods": 110,
  "minCount": null,
  "mode": "User",
  "name": "batchprocpl",
  "nodeImageVersion": "AKSUbuntu-1604-2020.06.10",
  "nodeLabels": null,
  "nodeTaints": null,
  "orchestratorVersion": "1.17.9",
  "osDiskSizeGb": 128,
  "osType": "Linux",
  "provisioningState": "Succeeded",
  "proximityPlacementGroupId": null,
  "resourceGroup": "rg-akscostsaving",
  "scaleSetEvictionPolicy": null,
  "scaleSetPriority": null,
  "spotMaxPrice": null,
  "tags": null,
  "type": "Microsoft.ContainerService/managedClusters/agentPools",
  "upgradeSettings": {
    "maxSurge": null
  },
  "vmSize": "Standard_DS2_v2",
  "vnetSubnetId": null
}
```

Notice that the node pool `count` parameter value is 0 and that the `enableAutoScaling` value is set to `null` in the returned result. You'll have to increase the node count for this node pool manually when you ned to schedule workloads here, becaue node creation won't happen automatically.

### Configure the Kubernetes Context
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

In the output from the previous command, the node pool count is set to 0. You can confirm the available nodes in the cluster by running the `kubectl get nodes` command.

1. You run `kubect` to interact with your cluster's API server. You have to configure a Kubernetes cluster context to allow `kubectl` to connect. The context contains contains the cluster's address, a user, and a namespace. Run the `az aks get-credentials` command to configure the Kubernetes context:

    ```bash
    az aks get-credentials \
        --resource-group $RESOURCE_GROUP \
        --name $AKS_CLUSTER_NAME

    # if running in WSL
    cp /mnt/c/Users/{user}/.kube/config ~/.kube/config
    ```

2. Run `kubectl get nodes` to list the nodes in your node pools:

    NAME | STATUS | ROLES | AGE | VERSION
    -----|--------|-------|-----|--------
    aks-nodepool1-16523171-vmss000000 | Ready | agent | 35m | v1.25.4
    aks-nodepool1-16523171-vmss000001 | Ready | agent | 35m | v1.25.4

    Notice that even though the `az aks nodepool list` command lists two node pols, there are only two nodes available in the cluster, and both are from `nodepool1`.

To optimize costs on AKS when you manage workload demands dirctly, a good strategy is to:

* Manually scale the node count in node pools.
* Scale expensive, NV-based user node pools to zero.

## Spot VM
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

A *spot virtual machine* is a VM that gives you access to unused Azure compute capacity at deep discounts. Spot VMs replace the existing, low-priority VMs in Azure. You can use spot VMs to run workloads that include:

* High-performance computing scenarios, batch processing, or visual-rendering applications.
* Large-scale, stateless applications
* Developer/test environments, including continuous integration (CI) and continuous delivery (CD) workloads.

### Spot VM Availability
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Spot VM availability depends on factors such as capacity, size, region, and time of day. Azure allocates VMs only if capacity is available. As a result, there's no service-level agreement (SLA) for these types of VMs and they offer no high-availability guarantees.

### Spot VM Eviction Policy
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

The default eviction policy for spot VMs is **Deallocate**. Azure will evict spot VMs with 30 seconds of notice when capacity in a region becomes limited. A VM that's set with the **Deallocate** policy moves to the stopped-deallocated state when evicted. You can redeploy an evicted VM when spot capacity becomes available again. A deallocated VM is still counted toward your spot virtual CPU (vCPU) quota, and charges for the underlying allocated disks still apply.

### Spot VM Scale Set
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

A spot virtual machine scale set is a virtual machine scale set that supports Azure spot VMs. These VMs behave the same way as normal spot VMs, but with one difference: when you use virtual machine scale set support for spot VMs in Azure, you choose between two eviction policies:

* **Deallocate**: The Deallocate policy functions exactly as described above.
* **Delete**: The Delete policy allows you to avoid the cost of disks and hitting quota limits. With the Delete eviction policy, evicted VMs are deleted together with their underlying idsks. The scale set's autoscaling feature can now automatically try to compensate for the eviction of VMs by creating new VMs. Although the creation of VMs isn't guaranteed, the evicted VMs don't count toward your vCPU quota or incur costs for underlying disks.

A best practice is to use the autoscale feature only when you set the eviction policy to **Delete** on the scale set.

## Spot Node Pool
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

A *spot node pool* is a user node pool that uses a spot virtual machine scale set. AKS supports spot VMs when you:

* Need to create user node pools.
* Want the cost benefits offered by virtual machine scale set support for Azure spot VMs.

Use spot node pools to:

* Take advantatge of unused capacity in Azure.
* Use scale set features with the Delete eviction policy.
* Define the maximum price you want to pay per hour.
* Enable the recommended AKS Kubernetes cluster autoscaler when using spot node pools.

For example, to support a batch-processing, you can create a spot user node pool and enable the cluster autoscaler. You teh nconfigure the horizontal pod scaler to deploy additional batch-processing services to match resource demands.

As the demand for nodes increases, the cluster autoscaler can scale the number of nodes up and down in the spot node pool. If node evictions happen, the cluster autoscaler keeps trying to scale up the node count if additional nodes are still needed.

### Spot Pool Limitations
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Before you decide to add a spot user node pool to your AKS cluster, consider the following limitations:

* The underlying spot scale set is deployed only to a single fault domain and offers no high-availability guarantees.
* The AKS cluster needs multiple node-pool support to be enabled.
* You can use spot node pools only as user node pools.
* Spot node pools can't be upgraded.
* The creation of spot VMs isn't guaranteed. The creation of spot nodes depends on capacity and quota availability in the cluster's deployed Azure region.

Remember tha tspot node pools should be used only for workloads that can be interrupted.

## Add a Spot Node Pool to an AKS Cluster
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

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
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

The `--priority` parameter is set to `Regular` by default for a new node pool. Set th evalue to `Spot` to indicate that the new pool you're creating is a spot node pool. This value can't be changed after creation.

### Eviction Policy
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

A spot node pool must use a virtual machine scale set. The spot node pool uses a spot scale set. Set `--eviction-policy` to `Delete` to allow the scale set to remove both the node and the underlying, allocated disk used by the node. This value can't be changed after creation.

You can set the eviction policy to `Deallocate`; however, when these nodes are evicted, they'll count against your compute quota and impact later scaling or upgrading of the cluster.

### Maximum Price for Spot Node
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Spot node pools enable you to optimize costs by setting the maximum amount that you're willing to pay per spot node per hour. To set your safe ammount, use the `--spot-max-price` parameter. Newly created spot nodes are evicted when this value is reached. You can set this value to any positive amount up to five decimal places, or set it to -1. Setting the `--spot-max-price` value to -1 affects your node pool in the following ways:

* Nodes won't be evicted based on the node's price.
* The cost for new nodes will be the current price for spot nodes or the price for a standard node, whichever is lower

For example, if you set the value to 0.98765, the maximum price for a node will be USD0.98765 per hour. When the node's consuption exceeds this amount, it's evicted.

### Enable the Cluster Autoscaler
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

We recommend that you enable the cluster autoscaler by using the `--enable-cluster-autoscaler` parameter. If you don't use the cluster autoscaler, you risk the node count dropping to zero in the node pool as nodes are evicted because of Azure capacity constraints.

### Minimum Node Count
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Set the minimum node count to a value between 1 and 100 by using the `--min-count` parameter. A minimum node count is required when you enable the cluster autoscaler.

### Maximum Node Count
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Set the maximum node count to a value between 1 and 100 by using the `--max-count` parameter. A maximum node count is required when you enable the cluster autoscaler.

## Deploy Pods to Spot Node Pools
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

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
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

A taint is applied to a node to indicate that only specific pods can be scheduled on it. Spot nodes are configured with a label set to `kubernetes.azure.com/scalesetpriority:spot`.

### What is Toleration?
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Toleration is a specification applied to a pod to allow, but not require, a pod to be scheduled on a node with corresponding taint. Spot nodes are configured with a node taint set to `kubernetes.azure.com/scalesetpriority=spot:NoSchedule`.

### What is Node Affinity?
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

You use node affinity to describe which pods are scheduled on a node. Affinity is specified by using labels defined on the node. For example, in AKS, system pods are configured wiht anti-affinity towards spot nodes to prevent the pods from being scheduled on these nodes.

### Define Toleration in a Pod Manifest File
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

You specify node taint toleration by creating a `tolerations` dictionary entry in your workload manifest file. In this dictionary, you set the following properties for each node taint the workload has to tolerate in this section:

Property | Description
---------|------------
`key` | Identifies a node taint key-value pair specified on the node. For example, on a spot node pool, the key-value pair is `kubernetes.azure.com/scalesetpriority:spot`. The key is `kubernetes.azure.com/scalesetpriority`.
`operator` | Allows the toleration to match a taint. The default operator is `Equal`. You can also specify `Exists` to match toleration. However, when you use `Exists`, you don't specify the following property (`value`).
`value` | Represents the value part of the node taint key-value pair that is specified on the node. For example, on a spot node pool with a key-value pair of `kubernetes.azure.com/scalesetpriority:spot`, the value is `spot`.
`effect` | Indicates how the scheduling of a pod is handled in the system. There are three options: `NoSchedule`, `PreferNoSchedule`, and `NoExecute`. `NoSchedule` ensures that the system won't schedule the pod. `PreferNoSchedule` allows the system to try not to schedule the pod. `NoExecute` either evicts pods that are already running on the tainted node or doesn't schedule the pod at all.

### Define Node Affinity in a Pod Manifest File
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

You specify affinity by creating an `affinity` entry in your workload manifest file. In this entry, you set the following properties for each node label that a workload must match:

Property | Description
---------|------------
`nodeAffinity` | Describes node affinity scheduling rules for the pod.
`requiredDuringSchedulingIgnoredDuringExecution` | If the affinity requirements specified by this field aren't met at scheduling time, the pod won't be scheudled on the node. If the affinity requirements specified by this field cease to be met at some point during pod execution (for example, due ot an update), the system may or may not try to eventually evict the pod from its node.
`nodeSelectorTerms` | A list of node selector terms. The terms are ORed rather than ANDed.
`matchExpressions` | A list of node selector requirements by node's labels.
`key` | The label key that the selector applies to. The key is `kubernetes.azure.com/scalesetpriority`.
`operator` | Represents a key's relationship to a set of values. Valid operators are `In`, `NotIn`, `Exists`, `DoesNotExist`, `Gt` and `Lt`.
`values` | Represents the value part of the node label key-value pair that is specified on the node. On a spot node pool with a key-value pair of `kubernetes.azure.com/scaelsetpriority:spot`, teh value is spot.