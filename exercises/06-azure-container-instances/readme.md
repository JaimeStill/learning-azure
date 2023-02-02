# Azure Container Instances

Containers offer a standardized and repeatable way to package, deploy, and manage cloud applications. Azure Container Instances let you run a container in Azure without managing virtual machines and without a higher-level service.

Azure Container Instances are useful for scenarios that can operate in isolated containers, including simple applications, task automation, and build jobs. Here are some benefits:

* **Fast startup**: Launch containers in seconds.
* **Per second billing**: Incur costs only while the container is running.
* **Hypervisor-level security**: Isolate your application as completely as it would be in a VM.
* **Custom sizes**: Specify exact values for CPU cores and memory.
* **Persistent storage**: Mount Azure Files shares directly to a container to retrieve and persist state.
* **Linux and Windows**: Schedule both Windows and Linux containers using the same API.

For scenarios where you need full container orchestration, including service discovery across multiple containers, automatic scaling, and coordinated application upgrades, Azure Kubernetes Service (AKS) is recommended.

## Run Azure Container Instances

Here, you'll create a container in Azure and expose it to the Internet with a fully qualified domain name (FQDN).