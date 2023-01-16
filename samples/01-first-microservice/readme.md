# Microservices Tutorial

**Tutorials**  

* [Introduction to Docker Containers](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/)
* [Your First Microservice](https://dotnet.microsoft.com/en-us/learn/aspnet/microservice-tutorial)
* [Deploy a Microservice to Azure](https://dotnet.microsoft.com/en-us/learn/aspnet/deploy-microservice-tutorial)

**Contents**  

* [Intro to Docker Notes](#intro-to-docker-notes)
   * [How to Build an Image](#how-to-build-an-image)
* [Docker](#docker)
* [Azure CLI](#azure-cli)
    * [Install](#install)
    * [Configure](#configure)
    * [Create AKS Cluster](#create-aks-cluster)
    * [Deploy](#deploy)
    * [Test Deployed Service](#test-deployed-service)
    * [Scale the Service](#scale-the-service)
    * [Delete All Resources](#delete-all-resources)

## Intro to Docker Notes
[Back to Top](#microservices-tutorial)

**Docker Container Lifecycle**  

![docker-container-lifecycle](https://user-images.githubusercontent.com/14102723/211899857-e41d4c91-ed09-40c7-826c-ee8249fe408c.png)

```bash
# view images in the local Docker image registry
docker images

# remove an image from the local Docker image registry
docker rmi <tag>:<version>

# place a container in the run state
docker run -d tmp-ubuntu

# place a container in the pause state
docker pause happy_wilbur

# unpause a container
docker unpause happy_wilbur

# restart a container
docker restart happy_wilbur

# place a container in the stopped state
docker stop happy_wilbur

# terminate a container
docker kill happy_wilbur

# remove one or more containers
docker rm happy_wilbur

# list running containers in all states
docker ps -a
```

A [`Dockerfile`](./Dockerfile) is a text file that contains the instructions used to build and run a Docker image. The following aspects of the image are defined:

* The base or parent image used to create the new image
* Commands to update the base OS and install additional software
* Build artifacts to include, such as a developed application
* Services to expose, such as storage and network configuration
* Command to run when the container is launched

***Example***

```Dockerfile
# Step 1: Specify the parent image for the new image
FROM ubuntu:18.04

# Step 2: Update OS packages and install additional software
RUN apt -y update && apt install -y wget nginx software-properties-common apt-transport-https \
    && wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.db \
    && add-apt-repository universe \
    && apt -y update \
    && apt install -y dotnet-sdk-3.0
    
# Step 3: Configure Nginx environment
CMD service nginx start

# Step 4: Configure Nginx environment
COPY ./default /etc/nginx/sites-available/default

# Step 5: Configure work directory
WORKDIR /app

# Step 6: Copy website code to container
COPY ./website/. .

# STEP 7: Configure network requirements
EXPOSE 80:8080

# Step 8: Define the entry point of the process that runs in the container
ENTRYPOINT ["dotnet", "website.dll"]
```

### How to Build an Image
[Back to Top](#microservices-tutorial)

```bash
# Build an image from the above Dockerfile
docker build -t temp-ubuntu .
```

**Generated Output:**

```bash
Sending build context to Docker daemon  4.69MB
Step 1/8 : FROM ubuntu:18.04
 ---> a2a15febcdf3
Step 2/8 : RUN apt -y update && apt install -y wget nginx software-properties-common apt-transport-https && wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && dpkg -i packages-microsoft-prod.deb && add-apt-repository universe && apt -y update && apt install -y dotnet-sdk-3.0
 ---> Using cache
 ---> feb452bac55a
Step 3/8 : CMD service nginx start
 ---> Using cache
 ---> ce3fd40bd13c
Step 4/8 : COPY ./default /etc/nginx/sites-available/default
 ---> 97ff0c042b03
Step 5/8 : WORKDIR /app
 ---> Running in 883f8dc5dcce
Removing intermediate container 883f8dc5dcce
 ---> 6e36758d40b1
Step 6/8 : COPY ./website/. .
 ---> bfe84cc406a4
Step 7/8 : EXPOSE 80:8080
 ---> Running in b611a87425f2
Removing intermediate container b611a87425f2
 ---> 209b54a9567f
Step 8/8 : ENTRYPOINT ["dotnet", "website.dll"]
 ---> Running in ea2efbc6c375
Removing intermediate container ea2efbc6c375
 ---> f982892ea056
Successfully built f982892ea056
Successfully tagged temp-ubuntu:latest
```

## Docker
[Back to Top](#microservices-tutorial)

```bash
docker build -t mymicroservice .
```

The `docker build` command uses the [`Dockerfile`](./Dockerfile) to build a Docker image.

* The `-t mymicroservice` parameter tells it to tag (or name) the image as `mymicroservice`.
* The final parameter tells it which directory to use to find the `Dockerfiles` (. specifies the current directory).
* This command will download and build all dependencies to create a Docker image and may take some time.

```bash
# see a list of all images available on your machine
docker images
```

```bash
# run the app in a container
docker run -it --rm -p 3000:80 --name mymicroservicecontainer mymicroservice
```

```bash
# view your container running
docker ps
```

> [Docker Hub](https://hub.docker.com)  

```bash
# login to Docker Hub account
docker login
```

```bash
# Re-tag Docker image under username
docker tag mymicroservice jaimestill/mymicroservice
# Push image to Docker Hub
docker push jaimestill/mymicroservice
```

## Azure CLI

### Install
[Back to Top](#microservices-tutorial)

> [Install on Linux](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-linux?view=azure-cli-latest&pivots=apt)

**Download script file to view**

```bash
# install in one line
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# download install script to inspect (if curious)
curl -sL https://aka.ms/InstallAzureCLIDeb --output install-azure-cli.sh
```

### Configure
[Back to Top](#microservices-tutorial)

```bash
# login to azure subscription
az login --use-device-code

# install Azure Kubernetes Service CLI
sudo az aks install-cli

# only necessary if you don't have a resource group
az group create --name MyMicroserviceResources --location eastus

# list current resource groups
az group list

# list available locations in a table
az account list-locations -o table

# view azure subscriptions
az account list --all

# set a session subscription
az account set -s NAME_OR_ID
```

### Create AKS Cluster
[Back to Top](#microservices-tutorial)

```bash
# create AKS cluster in the resource group
az aks create --resource-group RESOURCE_GROUP --name MyMicroserviceCluster --node-count 1 --enable-addons http_application_routing --generate-ssh-keys

# Download credentials to deploy to AKS cluster
az aks get-credentials --resource-group RESOURCE_GROUP --name MyMicroserviceCluster
```

### Deploy
[Back to Top](#microservices-tutorial)

```bash
# create a deployment file
touch deploy.yaml
```

> [deploy.yaml](./deploy.yaml)

```bash
# Deploy based on deploy.yaml
kubectl apply -f deploy.yaml
```

### Test Deployed Service
[Back to Top](#microservices-tutorial)

```bash
# after starting, http://[external-ip-address]/WeatherForecast
kubectl get service mymicroservice --watch
```

### Scale the Service
[Back to Top](#microservices-tutorial)

```bash
# scale to two instances
kubectl scale --replicas=2 deployment/mymicroservice
```

### Delete All Resources
[Back to Top](#microservices-tutorial)

```bash
az group delete -n MyMicroserviceResources
```
