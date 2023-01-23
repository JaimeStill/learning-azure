# Explore the Functionality of a Kubernetes Cluster

* [Microk8s](#microk8s)
* [Install Microk8s with WSL](#install-microk8s-with-wsl)
* [Prepare the Cluster](#prepare-the-cluster)
* [Explore the Kubernetes Cluster](#explore-the-kubernetes-cluster)
    * [Display Cluster Node Information](#display-cluster-node-information)
    * [Install a Web Server on a Cluster](#install-a-web-server-on-a-cluster)
    * [Test the Website Installation](#test-the-website-installation)
    * [Scale a Web Server Deployed on a Cluster](#scale-a-web-server-deployed-on-a-cluster)
* [Uninstall Microk8s](#uninstall-microk8s)

Your goal in this exercise is to explore a Kubernetes installation with a single-node cluster. You're going to configure a *Microk8s* environment that's easy to setup and teardwon. Then, you'll deploy an NGINX website and scale it out to multiple instances. Finally, you'll go through the steps to delete the running pods and clean up the cluster.

Keep in mind that there are other options, such a MiniKube and Kubernetes support in Docker, to do the same.

## Microk8s
[Back to Top](#explore-the-functionality-of-a-kubernetes-cluster)

An option for deploying a single-node Kubernetes cluster as a single package to target workstations and Internet of Things (IoT) devices. Canonical, the creator of Ubuntu Linux, originally developed and maintains Microk8s.

## Install Microk8s with WSL
[Back to Top](#explore-the-functionality-of-a-kubernetes-cluster)

* [Enable systemd in WSL](https://devblogs.microsoft.com/commandline/systemd-support-is-now-available-in-wsl/#set-the-systemd-flag-set-in-your-wsl-distro-settings)
* `sudo snap install microk8s --classic`

## Prepare the Cluster
[Back to Top](#explore-the-functionality-of-a-kubernetes-cluster)

1. Check the status of the installation:

    ```bash
    sudo microk8s.status --wait-ready
    ```

2. Enable the DNS, Dashboard, and Registry add-ons.

    Add-ons | Purpose
    --------|--------
    `DNS` | Deploys the `coreDNS` service.
    `Dashboard` | Deploys the `kubernetes-dashboard` service and several other services that support its functionality. It's a general-purpose, web-based UI for Kubernetes clusters.
    `Registry` | Deploys a private registry and several services that support its functionality. To store private containers, use this registry.

    ```bash
    sudo microk8s.enable dns dashboard registry
    ```

## Explore the Kubernetes Cluster
[Back to Top](#explore-the-functionality-of-a-kubernetes-cluster)

Microk8s provides a version of `kubectl` that you can use to interact with your new Kubernetes cluster. This copy of `kubectl` enables you to have a parallel installation of antoher system-wide `kubectl` instance without affecting its functionality.

1. Run the `snap alias` command to alias `microk8s.kubectl` to `kubectl`. This step simplifies usage.

    ```bash
    sudo snap alias microk8s.kubectl kubectl
    ```

### Display Cluster Node Information
[Back to Top](#explore-the-functionality-of-a-kubernetes-cluster)

Recall that a Kubernetes cluster exists out of control planes and worker nodes. Let's explore the new cluster to see what's installed.

1. Check the nodes that are running in your cluster.

    You know that Microk8s is a single-node cluster installation, so you expect to see only one node. Keep in mind, though, that this node is both the control plane and a worker node in the cluster. Confirm this configuration by running the `kubectl get nodes` command. To retrieve information about all the resources in your cluster, run the `kubectl get` command.

    ```bash
    sudo kubectl get nodes
    ```

    Output:

    NAME | STATUS | ROLES | AGE | VERSION
    -----|--------|-------|-----|--------
    microk8s-vm | Ready | \<none\> | 06m | v1.26.0

    You can get more information for the specific resource that's requested. For example, let's assume that you need to find the IP address of the node. To fetch extra information from the API server, run the `-o wide` parameter:

    ```bash
    sudo kubectl get nodes -o wide
    ```

    Output:

    NAME | STATUS | ROLES | AGE | VERSION | INTERNAL-IP | EXTERNAL-IP | OS-IMAGE | KERNEL-VERSION | CONTAINER-RUNTIME
    -----|--------|-------|-----|---------|-------------|-------------|----------|----------------|------------------
    microk8s-vm | Ready | \<none\> | 19m | 1.26.0 | 172.31.217.225 | \<none\> | Ubuntu 22.04.1 LTS | 5.15.79.1-microsoft-standard-WSL2 | containerd://1.6.8

2. Explore the services running on your cluster. As with nodes, to find information about the services running on the cluster, run the `kubectl get` command.

    ```bash
    sudo kubectl get services -o wide
    ```

    Output:

    NAME | TYPE | CLUSTER-IP | EXTERNAL-IP | PORT(S) | AGE | SELECTOR
    -----|------|------------|-------------|---------|-----|---------
    kubernetes | ClusterIP | 10.152.183.1 | \<none\> | 443/TCP | 23m | \<none\>

    Only one services is listed. You installed add-ons on th cluster earlier, and you expect to see these services as well.

    The reason for the single service listing is that Kubernetes uses a concept call *namespaces*. To logically divide a cluster into multiple virtual clusters, use namespaces.

    To fetch all services in all namespaces, pass the `--all-namespaces` parameter.

    ```bash
    sudo kubectl get services -o wide --all-namespaces
    ```

    Notice that you have three namespaces in your cluster. They're the default, `container-registry`, and `kube-system` namespaces. Here, you can see the `registry`, `kube-dns`, and `kubernetes-dashboard` instances that you installed. You'll also see the supporting services that were installed alongside some of the add-ons.

    Output:

    NAMESPACE | NAME | TYPE | CLUSTER-IP | EXTERNAL-IP | PORT(S) | AGE | SELECTOR
    ----------|------|------|------------|-------------|---------|-----|---------
    default | kubernetes | ClusterIP | 10.152.183.1 | \<none\> | 443/TCP | 27m | \<none\>
    kube-system | kube-dns | ClusterIP | 10.152.183.10 | \<none\> | 53/UDP,53/TCP,9153/TCP | 21m | k8s-app=kube-dns
    kube-system | metrics-server | ClusterIP | 10.152.183.44 | \<none\> | 443/TCP | 21m | k8s-app=metrics-server
    kube-system | kubernetes-dashboard | ClusterIP | 10.152.183.248 | \<none\> | 443/TCP | 21m | k8s-app=kubernetes-dashboard
    kube-system | dashboard-metrics-scraper | ClusterIP | 10.152.183.236 | \<none\> | 8000/TCP | 21m | k8s-app-dashboard-metrics-scraper
    container-registry | registry | NodePort | 10.152.183.73 | \<none\> | 5000:32000/TCP | 21m | app=registry

### Install a Web Server on a Cluster
[Back to Top](#explore-the-functionality-of-a-kubernetes-cluster)

You can use pod manifest files to describe your pods, replica sets, and deployments to define workloads. Because these files haven't been covered in detail, you'll use `kubectl` to pass information directly to the API server.

Even though the use of `kubectl` is handy, using manifest files is a best practice. Manifest files enable you to roll forward or roll back deploymetns with ease in your cluster. THese files also help document the configuration of a cluster.

1. To create an NGINX deployment, run the `kubectl create deployment` command:

    ```bash
    sudo kubectl create deployment nginx --image=nginx
    ```

2. To fetch the information about your deployment, run:

    ```bash
    sudo kubectl get deployments
    ```

    Output:

    NAME | READY | UP-TO-DATE | AVAILBLE | AGE
    -----|-------|------------|----------|----
    nginx | 1/1 | 1 | 1 | 45s

3. The deployment created a pod. To fetch information about your cluster's pods, run:

    ```bash
    sudo kubectl get pods
    ```

    Output:

    NAME | READY | STATUS | RESTARTS | AGE
    -----|-------|--------|----------|----
    nginx-748c667d99-mjpch | 1/1 | Running | 0 | 2m27s

### Test the Website Installation

1. Find the address of the pod:

    ```bash
    sudo kubectl get pods -o wide
    ```

    Output:

    NAME | READY | STATUS | RESTARTS | AGE | IP | NODE | NOMINATED NODE | READINESS GATES
    -----|-------|--------|----------|-----|----|------|----------------|----------------
    nginx-748c667d99-mjpch | 1/1 | Running | 0 | 4m42s | 10.1.18.9 | microk8s-vm | \<none\> | \<none\>

2. To access the website, run:

    ```bash
    wget 10.1.18.9
    ```

    See [index.html](./assets/index.html).

### Scale a Web Server Deployed on a Cluster
[Back to Top](#explore-the-functionality-of-a-kubernetes-cluster)

To scale number of replicas in your deployment, run the `kubectl scale` command. Specify the number of replicas you need and the name of the deployment.

1. To scale the total of NGINX pods to three, run:

    ```bash
    sudo kubectl scale --replicas=3 deployments/nginx
    ```

    The scale command enables you to scale the instacne count up or down.

2. To check the number of running pods, run:

    ```bash
    sudo kubectl get pods -o wide
    ```

    Output:

    NAME | READY | STATUS | RESTARTS | AGE | IP | NODE | NOMNIATED NODE | READINESS GATES
    -----|-------|--------|----------|-----|----|------|----------------|----------------
    nginx-748-c667d99-mjpch | 1/1 | Running | 0 | 18m | 10.1.18.9 | microk8s-vm | \<none\> | \<none\>
    nginx-748-c667d99-665fr | 1/1 | Running | 0 | 18m | 10.1.18.10 | microk8s-vm | \<none\> | \<none\>
    nginx-748-c667d99-qpknh | 1/1 | Running | 0 | 18m | 10.1.18.11 | microk8s-vm | \<none\> | \<none\>


You would need to apply several additional configurations to the cluster to effectively expose your website as a public-facing website. Examples include installing a load balancer and mapping node IP addresses. This type of configuration forms part of advanced aspects that you'll explore in the future.

## Uninstall Microk8s

1. To remove the add-ons from the cluster, run:

    ```bash
    sudo microk8s.disable dashboard dns registry
    ```

2. To remove microk8s from the VM, run:

    ```bash
    sudo snap remove microk8s
    ```