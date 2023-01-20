# Optimize Compute Costs on Azure Kubernetes Service

## Node Pools

A node pool describes a group of nodes with the same configuration in an AKS cluster. These nodes contain the underlying VMs that run your applications. You can create two types of node pools on an AKS-managed Kubernetes cluster:

* System node pools
* User node pools

### System Node Pools

System node pools host critical system pods that make up the control plane of your cluster. A system node pool allows the use of Linux only as the node OS and runs only Linux-based workloads. Nodes in a system node pool are reserved for system workloads and normally not used to run custom workloads. Every AKS cluster must contain at least one system node pool with at least one node, and you must define the underlying VM sizes for nodes.

### User Node Pools

User node pools support your workloads, and you can specify Windows or Linux as the node operating system. You can also define the underlying VM sizes for nodes and run specific workloads. For example, a solution has a batch-processing service that you deploy to a node pool configured with general-purpose VMs. The new predictive-modeling service requires higher-capacity, GPU-based VMs. You decide to configure a separate node pool and configure it to use GPU-enabled nodes.