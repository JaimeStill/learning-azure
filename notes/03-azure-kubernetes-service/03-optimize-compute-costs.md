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