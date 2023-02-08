# Learning Azure

* [Exercises](./exercises/)
    * [App Service](./exercises/app-service/)
        * [Azure App Service](./exercises/app-service/01-azure-app-service/)
        * [Deploy and Run a Containerized Web App with Azure App Service](./exercises/app-service/02-app-service-containerized/)
    * [Docker](./exercises/docker/)
        * [Build a Containerized Web App with Docker](./exercises/docker/01-containerized-docker-app/)
        * [Azure Container Instances](./exercises/docker/02-azure-container-instances/)
        * [Azure Container Registry](./exercises/docker/03-azure-container-registry/)
    * [Kubernetes](./exercises/kubernetes/)
        * [Explore Kubernetes Cluster](./exercises/kubernetes/01-explore-kubernetes-cluster/)
        * [Deploy to AKS](./exercises/kubernetes/02-deploy-to-aks/)
        * [Optimize AKS Compute Costs](./exercises/kubernetes/03-optimize-aks-compute-costs/)
        * [Helm Package Management](./exercises/kubernetes/04-helm-package-management/)
    * [Redis](./exercises/redis/)
        * [Connect an App to Azure Cache for Redis Using .NET Core](./exercises/redis/01-connect-azure-cache-dotnet/)
        * [Optimize Your Web Applications By Caching Read-Only Data With Redis](./exercises/redis/02-cache-readonly-data-redis/)
        * [Work With Mutable and Partial Data in Azure Cache for Redis](./exercises/redis/03-redis-transactions/)
        * [Implement Pub/Sub and Streams in Azure Cache for Redis](./exercises/redis/04-redis-pub-sub/)
* [Notes](./notes)
    * [Azure Kubernetes Services](./notes/aks.md)
    * [Azure App Service](./notes/azure-app-service.md)
    * [Azure CLI](./notes/azure-cli.md)
    * [Codespaces](./notes/codespaces.md)
    * [Docker](./notes/docker.md)
    * [eShop Containers Architecture](./notes/eshop-containers-architecture.md)
    * [Helm](./notes/helm.md)
    * [Kubernetes](./notes/kubernetes.md)
* [Training](#training)
* [Links](#links)
    * [Related Technology Links](#related-technology-links)
    * [Architecture Patterns](#architecture-patterns)

## Training
[Back to Top](#learning-containers)

* [Intro to Kubernetes on Azure](https://learn.microsoft.com/en-us/training/paths/intro-to-kubernetes-on-azure/)
* [Architect Modern Applications in Azure](https://learn.microsoft.com/en-us/training/paths/architect-modern-apps/)
* [Your First Microservice](https://dotnet.microsoft.com/en-us/learn/aspnet/microservice-tutorial)
* [Deploy a Microservice to Azure](https://dotnet.microsoft.com/en-us/learn/aspnet/deploy-microservice-tutorial)
* [.NET Microservices](https://learn.microsoft.com/en-us/training/modules/dotnet-microservices)
* [Deploy a .NET Microservice to Kubernetes](https://learn.microsoft.com/en-us/training/modules/dotnet-deploy-microservices-kubernetes/)


## Links
[Back to Top](#learning-containers)

* [Docker](https://docs.docker.com/)
    * [Docker Hub](https://hub.docker.com)
    * [Dockerfile Reference](https://docs.docker.com/engine/reference/builder/)
    * [Dockerfile Best Practices](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/)
* [Kubernetes](https://kubernetes.io/docs/home/)
    * [Ingress Controllers](https://kubernetes.io/docs/concepts/services-networking/ingress-controllers/)
    * [Service Types](https://kubernetes.io/docs/concepts/services-networking/service/#publishing-services-service-types)
* [Azure Kubernetes Service (AKS)](https://learn.microsoft.com/en-us/azure/aks/)
    * [HTTP application routing](https://learn.microsoft.com/en-us/azure/aks/http-application-routing)
    * [AKS Ingress Controllers](https://learn.microsoft.com/en-us/azure/aks/ingress-tls?tabs=azure-cli)
    * [AKS Production Baseline](https://learn.microsoft.com/en-us/azure/architecture/reference-architectures/containers/aks/baseline-aks)
* [helm](https://helm.sh/)
    * [Template Functions and Pipelines](https://helm.sh/docs/chart_template_guide/functions_and_pipelines/)
    * [Template Function List](https://helm.sh/docs/chart_template_guide/function_list/)
* [Redis](https://redis.io/docs/)
    * [Pub/Sub](https://redis.io/docs/manual/pubsub/)
    * [Streams](https://redis.io/topics/streams-intro)


### Related Technology Links
[Back to Top](#learning-containers)

* [NGINX](https://www.nginx.com/)
* [Development Containers](https://containers.dev/)
* [Government Cloud Services in Compliance Scope](https://learn.microsoft.com/en-us/azure/azure-government/compliance/azure-services-in-fedramp-auditscope)
* [Open Policy Agent](https://www.openpolicyagent.org/docs/latest/)
* [multipass](https://github.com/canonical/multipass)
* [microk8s](https://microk8s.io/)
* [IdentityServer 4](https://identityserver4.readthedocs.io/en/latest/)
* [Azure Active Directory](https://azure.microsoft.com/en-us/products/active-directory/)
* [RabbitMQ](https://www.rabbitmq.com/)
* [Azure Service Bus](https://azure.microsoft.com/en-us/products/service-bus/)
* [Azure API Management](https://azure.microsoft.com/en-us/products/api-management/)
* [Seq](https://datalust.co/seq)
* [AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
* [Ansible](https://www.ansible.com/overview/how-ansible-works)
* [Terraform](https://www.terraform.io/use-cases/manage-kubernetes)
* [Vault](https://www.vaultproject.io/)
* [Portainer](https://www.portainer.io/)
* [Go Template Language](https://pkg.go.dev/text/template)
* [Sprig](https://github.com/Masterminds/sprig)

### Architecture Patterns
* [Backends-For-Frontends](https://learn.microsoft.com/en-us/azure/architecture/patterns/backends-for-frontends)
* [Gateway Aggregation](https://learn.microsoft.com/en-us/azure/architecture/patterns/gateway-aggregation)