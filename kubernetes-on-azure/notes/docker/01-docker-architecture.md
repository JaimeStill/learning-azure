# Docker Architecture

* [Container](#container)
    * [Containerization](#containerization)
* [Docker Engine](#docker-engine)
    * [Docker Client](#docker-client)
    * [Docker Server](#docker-server)
    * [Docker Objects](#docker-objects)
* [Docker Hub](#docker-hub)
* [Software](#software)
* [Images](#images)
    * [Tags](#tags)
* [Host OS](#host-os)
* [Stackable Unification File System](#stackable-unification-file-system)
    * [Base Image](#base-image)
    * [Parent Image](#parent-image)
    * [Base vs. Parent Images](#base-vs-parent-images)
* [Dockerfile](#dockerfile)

![docker-architecture](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/2-docker-architecture.svg)

Docker is a containerization platform used to develop, ship, and run containers. Docker doesn't use a hypervisor, and you can run Docker on your desktop or laptop if you're developing and testing applications. The desktop version of Docker supports Linux, Windows, and MacOS. For production systems, Docker is available for server environments, including many variants of Linux and Microsoft Windows Server 2016 and above. Many clouds, including Azure, support Docker.

## Container
[Back to Top](#docker-architecture)

A loosely isolated environment that allows us to build and run software packages. These software packages include the code and all dependencies to run applications quickly and reliably on any computing environmente. We call these packages *container images*.

The container image becomes the unit we use to distribute our applications.

### Containerization

Software containerization is an OS virtualization method that is used to deploy and run containers without using a virtual machine (VM). Containers can run on physical hardware, in the cloud, VMs, and across multiple OSs.

## Docker Engine
[Back to Top](#docker-architecture)

Consists of several componnets configured as a client-server implementation where the client and server run simultaneously on the same host. The client communicates with the server using a REST API, which enables the client to also communicate with a remote server instance.

### Docker Client
[Back to Top](#docker-architecture)

A command-line application named `docker` that provides us wiht a command line interface (CLI) to interact with the Docker server. The `docker` command uses the Docker REST API to send instructions to either a local or remote server and functions as the primary interface we use to manage our containers.

### Docker Server
[Back to Top](#docker-architecture)

A daemon named `dockerd`. The `dockerd` daemon responds to requests from the client via the Docker REST API and can interact with other daemons. The Docker server is also responsible for tracking the lifecycle of our containers.

### Docker Objects
[Back to Top](#docker-architecture)

There are several objects that you'll create and configure to support your container deployments. These include networks, storage volumes, plugins, and other service objects.

## Docker Hub
[Back to Top](#docker-architecture)

A Software as a Service Docker container registry. Docker registries are repositories that we use to store and distribute the container images we create. Docker Hub is the default public registry Docker uses for image management.

You can create and use a private Docker registry or use one of the many cloud provider options available. For example, you can use Azure Container Registry to store container images to use in several Azure cotnainer-enabled services.

## Software
[Back to Top](#docker-architecture)

The software packaged into a container isn't limited to the applications our developers build. When we talk about software, we refer to application code, system packages, binaries, libraries, configuration files, and the operating system running in the container.

For example, assume we're developing an order tracking portal that our company's various outlets will use. We need to look at the complete stack of software that will run our web application. The application we're building is a .NET Core MVC app, and we plan to deploy the application using Nginx as a reverse proxy server on Ubuntu Linux. All of these software components form part of the container image.

## Images
[Back to Top](#docker-architecture)

A portable package that contains software. It's this image that, when run, becomes our container. The container is the in-memory instance of an image.

A container image is immutable. Once you've built an image, the image can't be changed. The only way to change an image is to create a new image. This feature is our guarantee that the image we use in production is the same image used in development and QA.

### Tags

A text string that is used to version an image. When building an image, we name and optionally tag the image using the `-t` command flag, with the convention *{name}:{tag}*. An image is labeled with the `latest` tag if you don't specify a tag (i.e. - *temp-ubuntu:latest*).

A single image can have multiple tags assigned to it. By convention, the most recnet version of an image is assigned the *latest* tag and a tag that describes the image version number. When you release a new versino of an image, you can reassign the latest tag to reference the new image.

Here is another example. Suppose you want to use the .NET Core samples Docker images. Here we have four platform versions that we can choose from:

* `mcr.microsoft.com/dotnet/core/samples:dotnetapp`
* `mcr.microsoft.com/dotnet/core/samples:aspnetapp`
* `mcr.microsoft.com/dotnet/core/samples:wcfservice`
* `mcr.microsoft.com/dotnet/core/samples:wcfclient`

## Host OS
[Back to Top](#docker-architecture)

![host-os](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/3-container-scratch-host-os.svg)

The host OS is the OS on which the Docker engine runs. Docker containers running on Linux share the host OS kernel and don't require a container OS as long as the binary can access the OS kernel directly.

## Container OS
[Back to Top](#docker-architecture)

![container-os](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/3-container-ubuntu-host-os.svg)

The container OS is the OS that is part of the packaged image. We have the flexibility to include different versions of Linux or Windows OSs in a container. This flexibility allows us to access specific OS features or install additional software our applications may use.

> Windows containers need a container OS. The container depends on the OS kernel to manage services such as the file system, network management, process scheduling, and memory management.

The container OS is isolated from the host OS and is the environment in which we deploy and run our application. Combined with the image's immutability, this isolation means the environment for our application running in development is the same as in production.

## Stackable Unification File System
[Back to Top](#docker-architecture)

![unionfs-diagram](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/3-unionfs-diagram.svg)

`Unionfs` is used to create Docker images. `Unionfs` is a filesystem that allows you to stack several directories, called branches, in such a way that it appears as if the content is merged. However, the content is physically kept separate. `Unionfs` allows you to add and remove branches as you build out your file system.

Assume we're building an image for our web application from earlier. We'll layer the Ubuntu distribution as a base image on top of the boot file system. Next we'll install Nginx and our web app. We're effectively layering Nginx and the web app on top of the original Ubuntu image.

A final writeable layer is created once the container is run from the image. This layer however, does not persist when the container is destroyed.

### Base Image
[Back to Top](#docker-architecture)

An image that uses the Docker `scratch` image. The `scratch` image is an empty container that doesn't create a filesystem layer. This image assumes that the application you're going to run can directly use the host OS kernel.

### Parent Image
[Back to Top](#docker-architecture)

A container image from which you create images.

Instead of creating an image from `scratch` and then installing Ubuntu, we'll rather use an image already based on Ubuntu. We can even use an image that already has Nginx installed. A parent image usually includes a container OS.

### Base vs. Parent Images
[Back to Top](#docker-architecture)

Both image types allow us to create a reusable image. However, base images allow us more control over the contents of the final image. Recall that an image is immutable, you can only add to an image and not subtract.

## Dockerfile
[Back to Top](#docker-architecture)

A text file that contains thet instructions we use to build and run a Docker image. The following aspects of the iamge are defined:

* The base or parent image we use to create the new image
* Commands to update the base OS and install additional software
* Build artifacts to include, such as a developed application
* Services to expose, such as a storage and network configuration
* Command to run when the container is launched

```Dockerfile
# Step 1: Specify the parent image for the new image
FROM ubuntu:18.04

# Step 2: Update OS packages and install additional software
RUN apt -y update && apt-install -y wget nginx software-properties-common apt-transport-https \
    && wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
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
COPY ./website/ .

# Step 7: Configure network requirements
EXPOSE 80:8080

# Step 8: Define the entry point of the process that runs in the container
ENTRYPOINT ["dotnet", "website.dll"]
```

Each of the steps in the Dockerfile create a cached container image (using [`unionfs`](#stackable-unification-file-system)) as we build the final container image. These temporary images are layerd on top of the previous and presented as a single image once all steps complete.

The `ENTRYPOINT` in the file indicates which process will execute once we run a container from an image.

