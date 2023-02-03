# Optimize Compute Costs on Azure Kubernetes Service

## Configure Multiple Nodes and Enable Scale-to-Zero on an AKS Cluster
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Here, you'll create an Azure Kubernetes Service (AKS)-managed Kubernetes cluster. You'll configure the cluster to support multiple node pool and make sure the cluster allows you to scale the nodes in the node pools. Then you'll add a second node pool to support user workloads. Finally, you'll scale the node count in the second node pool to zero to reduce the cost of the node used in your AKS cluster.

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

    **Saved as [`get-kubernetes-version.sh`](./assets/get-kubernetes-version.sh)**  

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

    **Saved as [`create-aks.sh`](./assets/create-aks.sh)**  

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

    **Saved as [`add-aks-nodepool.sh`](./assets/add-aks-nodepool.sh)**  

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

**Saved as [`scale-node-to-zero.sh`](./assets/scale-node-to-zero.sh)  

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

## Configure Spot Node Pools with Cluster Autoscaler
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

Spot user node pools allow you to access unused Azure compute capacity at lower prices but still support high-performance computing scenarios.

Here, you'll add a spot user node pool with automatic scaling to reduce your cluster's operational costs where usage still varies but isn't as predictable. You'll also deploy a workload with node affinity enabled so that the pod is scheduled on nodes in the spot node pool.

### Enable Preview Features
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

1. Login to Azure CLI:

    ```bash
    az login
    ```

2. Register the **spotpoolpreview** flag by running the `az feature register` command. This command takes two parameters: *namespace* identifies the resource provider you're registering the feature with, and *name* identifies the feature:

    ```bash
    az feature register --namespace "Microsoft.ContainerService" --name "spotpoolpreview"
    ```

3. Check that registration is successful by querying the feature list table. Run the `az feature list` command to run the query. The feature's registration can take several minutes to finish, so you'll have to check the result periodically.

    ```bash
    az feature list -o table --query "[?contains(name, 'Microsoft.ContainerService/spotpoolpreview')].{Name:name,State:properties.state}"
    ```

4. When the feature registration is complete, run the `az provider register` command with the `--namespace` parameter to update the registration:

    ```bash
    az provider register --namespace Microsoft.ContainerService
    ```

### Install Azure CLI Preview Extensions
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

To work with this preview, you must use the aks-preview Azure CLI preview extensions for AKS. install version 0.4.0 of the Azure CLI preview extension by running the `az extension add` command:

```bash
az extension add --name aks-preview
```

You can check the installed version of the extensions if you've alreaady installed the preview version. Run the `az extension show` command to query the extension version:

```bash
az extension show --name aks-preview --query [version]
```

You can update the extension by running the `az extension update` command if you've previously installed the extension and need to update it to a newer version:

```bash
az extension update --name aks-preview
```

### Create a Spot Node Pool
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

You want to create a separate pool that supports the batch-processing service. This node pool is a spot node pool that runs the Delete eviction policy and a spot maximum price of -1.

1. Run the `az aks nodepool add` command as follows:

    ```bash
    az aks nodepool add \
        --ressource-group $RESOURCE_GROUP \
        --cluster-name $AKS_CLUSTER_NAME \
        --name batchprocpl2 \
        --enable-cluster-autoscaler \
        --max-count 3 \
        --min-count 1 \
        --priority Spot \
        --eviction-policy Delete \
        --spot-max-price -1 \
        --node-vm-size Standard_DS2_v2 \
        --no-wait
    ```

    > Keep in mind that this request might fail because of capacity restrictions in the location that you've selected.

2. Run the `az aks nodepool show` command to show the details of the new spot node pool for the batch-processing service:

    ```bash
    az aks nodepool show \
        --resource-group $RESOURCE_GROUP \
        --cluster-name $AKS_CLUSTER_NAME \
        --name batchprocpl2
    ```

    Output:

    ```
    {
        "agentPoolType": "VirtualMachineScaleSets",
        "availabilityZones": null,
        "count": 3,
        "enableAutoScaling": true,
        "enableNodePublicIp": false,
        "id": "/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourcegroups/rg-akscostsaving/providers/Microsoft.ContainerService/managedClusters/akscostsaving-17835/agentPools/batchprocpl2",
        "maxCount": 3,
        "maxPods": 110,
        "minCount": 1,
        "mode": "User",
        "name": "batchprocpl2",
        "nodeImageVersion": "AKSUbuntu-1604-2020.06.10",
        "nodeLabels": {
            "kubernetes.azure.com/scalesetpriority": "spot"
        },
        "nodeTaints": [
            "kubernetes.azure.com/scalesetpriority=spot:NoSchedule"
        ],
        "orchestratorVersion": "1.17.9",
        "osDiskSizeGb": 128,
        "osType": "Linux",
        "provisioningState": "Creating",
        "proximityPlacementGroupId": null,
        "resourceGroup": "akscostsavinggrp",
        "scaleSetEvictionPolicy": "Delete",
        "scaleSetPriority": "Spot",
        "spotMaxPrice": -1.0,
        "tags": null,
        "type": "Microsoft.ContainerService/managedClusters/agentPools",
        "upgradeSettings": {
            "maxSurge": null
        },
        "vmSize": "Standard_DS2_v2",
        "vnetSubnetId": null
    }
    ```

    A few values in this result are distinctly different from what you've seen in previous node pools:

    * The `enableAutoScaling` property value is set to `true`.
    * Both the `maxCount` and `minCount` values are set.
    * The `scaleSetEvictionPolicy` property is set to `Delete`.
    * The `scaleSetPriority` property is set to `Spot`.
    * The `spotMaxPrice` property is set to `-1`.
    * The `nodeLabels` and `nodeTaints` are applied to this node pool. you use these values to schedule workloads on the nodes in the node pool.

### Configure a Namespace
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

1. Run the `kubectl create namespace` command to create a namespace called `costsavings` for the application. You'll use this namespace to make it easier to select your workloads:

    ```bash
    kubectl create namespace costsavings
    ```

    You'll see a confirmation that the namespace was created:

    ```bash
    namespace/costsavings created
    ```

### Schedule a Pod with Spot Node Affinity
[Back to Top](#optimize-compute-costs-on-azure-kubernetes-service)

You can schedule a pod to run on a spot node by adding a toleration and an affinity to the pod's deployment manifest file. When the toleration and node affinity correspond with the taint and label applied to your spot nodes, the pod is scheduled on these nodes.

The nodes in a spot node pool are assigned a taint that equals `kubernetes.azure.com/scalesetpriority=spot:NoSchedule` and a label that equals `kubernetes.azure.com/scalesetpriority=spot`. Use the information in this key-value pair in the `tolerations` and `affinity` section of your workloads YAML manifest file. With the second batch-processing pool configured as a spot node pool, you can now create a deployment file to schedule workloads to run on it.

1. Create a manifest file for the Kubernetes deployment called [`spot-node-deployment.yaml`](./assets/spot-node-deployment.yaml):

    ```bash
    code spot-node-deployment.yaml
    ```

    Contents:

    ```yaml
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

2. Run the `kubectl apply` command to apply the configuration and deploy the application in the `costsavings` namespace:

    ```bash
    kubectl apply \
        --namespace costsavings
        -f spot-node-deployment.yaml
    ```

    Output:

    ```
    pod/nginx created
    ```

3. You acn fetch more information about the running pod by using the `-o wide` flag when running the `kubectl get pods` command. In this case, you want to see which node the pod is scheduled on. Make sure to query for pods in the `costsavings` namespace:

    ```bash
    kubectl get pods --namespace costsavings -o wide
    ```

    Output:

    NAME | READY | STATUS | RESTARTS | AGE | IP | NODE | NOMINATED NODE | READINESS GATES
    -----|-------|--------|----------|-----|----|------|----------------|----------------
    nginx | 1/1 | Running | 0 | 43s | 10.244.3.3 | aks-batchprocpl2-25254417-vmss000000 | \<none\> | \<none\>]

    Notice the name of the node, `aks-batchprocpl2-25254417-vmss000000`. This node is part of the `batchprocpl2` spot node pool that you created earler.

## Configure Azure Policy for Kubernetes on an AKS Cluster

In this exercise, you'll configure Azure Policy for Azure Kubernetes Service on your AKS cluster. You'll configure a **Kubernetes cluster containers CPU and memory resource limits should not exceed the specified limits** policy. Finally, you'll test that the policy denies the scheduling of workloads that exceed the policy's resource parameters.

### Enable the ContainerService and PolicyInsights Resource Providers

1. Sign into the Azure CLI:

    ```bash
    az login
    ```

2. Azure Policy for AKS requires the cluster version to be 1.14 or later. Run the following to validate your AKS cluster version:

    ```bash
    az aks list
    ```

    Make sure that the reported cluster version is 1.14 or later.

3. Register the Azure Kubernetes Service provider bu running the `az provider register` command:

    ```bash
    az provider register --namespace Microsoft.ContainerService
    ```

4. Register the Azure Policy provider by running the `az provider register` command:

    ```bash
    az provider register --namespace Microsoft.PolicyInsights
    ```

5. Enable the installation of the add-on by running the `az feature register` command:

    ```bash
    az feature register \
        --namesacpe Microsoft.ContainerService \
        --name AK-AzurePolicyAutoApprove
    ```

6. Check that the registration is successful by querying the feature-list table. Use the `az feature list` command to run the query. The feature's registration can take several minutes to finish, so you'll have to check the result periodically.

    ```bash
    az feature list \
        -o table \
        --query "[?contains(name, 'Microsoft.ContainerService/AKS-AzurePolicyAutoApprove')].{Name:name,State:properties.state}"
    ```

7. Run the `az provider register` command to propogate the update after you confirm that the feature-list query command shows 'Registered':

    ```bash
    az provider register -n Microsoft.ContainerService
    ```

### Enable the Azure Policy Add-on

1. Run the `az aks enable-addons` command to enable the `azure-policy` add-on for your cluster:

    ```bash
    az aks enable-addons \
        --addons azure-policy \
        --name $AKS_CLUSTER_NAME \
        --resource-group $RESOURCE_GROUP
    ```

2. Verify that the azure-policy pod is installed in the `kube-system` namespace and that the gatekeeper pod is installed in the `gatekeeper-system` namespace. To do so, run the following:

    ```bash
    kubectl get pods -n kube-system
    ```

    Output:

    NAME | READY | STATUS | RESTARTS | AGE
    -----|-------|--------|----------|----
    azure-policy-{uuid} | 1/1 | Running | 0 | 12m
    azure-policy-webhook-{uuid} | 1/1 | Running | 0 | 12m

    ```bash
    kubectl get pods -n gatekeeper-system
    ```

    Output:

    NAME | READY | STATUS | RESTARTS | AGE
    -----|-------|--------|----------|----
    gatekeeper-controller-manager-{uuid} | 1/1 | Running | 0 | 15m

3. Verify that the latest add-on is installed by running the `az aks show` command. This command retrieves the configuration information for your cluster:

    ```bash
    az aks show \
        --resource-group $RESOURCE_GROUP \
        --name $AKS_CLUSTER_NAME \
        -o table --query "addonProfiles.azurepolicy"
    ```

    Output:

    ```json
    {
        "config": null,
        "enabled": true,
        "identity": null
    }
    ```

### Assign a Built-In Policy Definition

To configure thet new Azure Policy, use the Policy service in the Azure portal.

1. Sign into the [Azure Portal](https://portal.azure.com).

2. Locate the **Policy** service in the Azure portal. To do so, in the search bar at the top of the portal, search for and select *Policy*.

3. Select the **Policy** service from the list of services, as shown here:

    ![policy-service-search-result](https://learn.microsoft.com/en-us/training/modules/aks-optimize-compute-costs/media/7-search-result.png)

4. In the left menu pane, under **Authoring**, select **Assignments**:

    ![policy-service-pane](https://learn.microsoft.com/en-us/training/modules/aks-optimize-compute-costs/media/7-assignment-option.png)

5. In the top menu bar, select **Assign policy**:

    ![assign-policy](https://learn.microsoft.com/en-us/training/modules/aks-optimize-compute-costs/media/7-assign-policy.png)

6. On the **Basics** tab, enter the following values for each setting to create your policy:

    Setting | Value
    --------|------
    Scope | Select the ellipsis button. The **Scope** pane appears. Under the **subscription**, select the subscription that holds your resource group. For **Resource Group**, select **akscostsavinggrp**, and then select **Select**.
    Exclusions | Leave empty.
    Policy Definition | Select the ellipsis button. The **Available Definitions** pane appears. In the **Search** box, filter selection by entering *CPU*. On the **Policy Definitions** tab, select the **Kubernetes cluster containers CPU and memory resource limits should not exceed the specified limits.** and then select **Select**.
    Assignment Name | Accept default.
    Description | Leave empty.
    Policy Enforcement | Make sure this option is set to **Enabled**.
    Assigned By | Accept default.

    Example completed **Basics** tab:

    ![assign-policy-basics](https://learn.microsoft.com/en-us/training/modules/aks-optimize-compute-costs/media/7-complete-basic-tab.png)

7. Select the **Parameters** tab to specify the parameters for the policy.

8. Set the following values for each of the parameter settings:

    Setting | Value
    --------|------
    Max allowed CPU units | Set the value to **200m**. The policy matches this value to both the workload resource-request value and the workload limit value specified in the workload's manifest file.
    Max allowed memory bytes | Set the value to 256Mi. The policy matches this value to both the workload resource-request value adn the workload limit value specified in the workload's manifest file.

    Example completed **Parameters** tab:

    ![assign-policy-parameters](https://learn.microsoft.com/en-us/training/modules/aks-optimize-compute-costs/media/7-complete-parameters-tab.png)

9. Select the **Remediation** tab. In this tab, you select how the new policy impacts resources that already exist. By default, only newly created resources are effected by the new policy. Leave the default configuration as it is on this tab.

    Example completed **Remediation** tab:

    ![assign-policy-remediation](https://learn.microsoft.com/en-us/training/modules/aks-optimize-compute-costs/media/7-complete-remediation-tab.png)

10. Select the **Review + create** tab. Review the values you've chosen, then select **Create**.

> If you're using an existing AKS cluster, the policy assignment may take about 15 minutes to apply.

### Test Resource Requests

The final step is to test the new policy. You'll deploy a test workload that includes resource requests and limits that violate the new policy.

1. Create a manifest file for the Kubernetes deployment named [`test-policy.yaml`](./assets/test-policy.yaml):

    ```yaml
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
        imagePullRequest: IfNotPresent
        resources:
          requests:
            cpu: 500m
            memory: 256Mi
          limits:
            cpu: 1000m
            memory: 500Mi
    ```

2. Run the `kubectl apply` command to apply the configuration and deploy the application in the `costsavings` namespace:

    ```bash
    kubectl apply \
        --namespace costsavings \
        -f test-policy.yaml
    ```

    Output:

    ```
    Error from server (
    [denied by azurepolicy-container-limits-52f2942767eda208f8ac3980dc04b548c4a18a2d1f7b0fd2cd1a7c9e50a92674] container <nginx> memory limit <500Mi> is higher than the maximum allowed of <256Mi>
    [denied by azurepolicy-container-limits-52f2942767eda208f8ac3980dc04b548c4a18a2d1f7b0fd2cd1a7c9e50a92674] container <nginx> cpu limit <1> is higher than the maximum allowed of <200m>)
    : error when creating "test-deploy.yml"
    : admission webhook "validation.gatekeeper.sh" denied the request: 
    [denied by azurepolicy-container-limits-52f2942767eda208f8ac3980dc04b548c4a18a2d1f7b0fd2cd1a7c9e50a92674] container <nginx> memory limit <500Mi> is higher than the maximum allowed of <256Mi>
    [denied by azurepolicy-container-limits-52f2942767eda208f8ac3980dc04b548c4a18a2d1f7b0fd2cd1a7c9e50a92674] container <nginx> cpu limit <1> is higher than the maximum allowed of <200m>
    ```

    Notice how the admission webhook, `validation.gatekeeper.sh`, denied the request to schedule the pod.

3. Open the manifest file and fix the resource request:

    **Saved as [`test-policy-fixed.yaml`](./assets/test-policy-fixed.yaml)**  

    ```yaml
    resources:
      requests:
        cpu: 200m
        memory: 256Mi
      limits:
        cpu: 200m
        memory: 256Mi
    ```

4. Run the `kubectl apply` command to apply the configuration and deploy the application in the `costsavings` namespace:

    ```bash
    kubectl apply \
        --namespace costsavings \
        -f test-policy-fixed.yaml
    ```

    Output:

    ```
    pod/nginx created
    ```

5. Run the `kubectl get pods` command to view the newly created popd. Make sure to query for pods in the `costsavings` namespace.

    ```bash
    kubectl get pods --namespace costsavings
    ```

    Output:

    NAME | READY | STATUS | RESTARTS | AGE
    -----|-------|--------|----------|----
    nginx | 1/1 | Running | 0 | 50s

## Clean Up Resources

1. List the Resource Groups in the Azure CLI:

    ```bash
    az group list -o table
    ```

    Output:

    NAME | LOCATION | STATUS
    -----|----------|-------
    **rg-akscostsaving** | eastus | Succeeded

2. Delete the **rg-akscostsaving** resource group:

    ```bash
    az group delete -n rg-akscostsaving -y
    ```

3. Run the `kubectl config delete-context` command to remove the deleted cluster's context:

    ```bash
    kubectl config delete-context akscostsaving-{number}
    ```

    Output:

    ```
    delete context akscostsaving-{number} from /home/user/.kube/config
    ```