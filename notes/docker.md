# Docker

![docker-architecture](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/2-docker-architecture.svg)

## Architecture
[Back to Top](#docker)

Docker is a containerization platform used to develop, ship, and run containers. Docker doesn't use a hypervisor, and you can run Docker on your desktop or laptop if you're developing and testing applications. The desktop version of Docker supports Linux, Windows, and MacOS. For production systems, Docker is available for server environments, including many variants of Linux and Microsoft Windows Server 2016 and above. Many clouds, including Azure, support Docker.

### Container
[Back to Top](#docker)

A loosely isolated environment that allows us to build and run software packages. These software packages include the code and all dependencies to run applications quickly and reliably on any computing environmente. We call these packages *container images*.

The container image becomes the unit we use to distribute our applications.

### Containerization

Software containerization is an OS virtualization method that is used to deploy and run containers without using a virtual machine (VM). Containers can run on physical hardware, in the cloud, VMs, and across multiple OSs.

### Docker Engine
[Back to Top](#docker)

Consists of several componnets configured as a client-server implementation where the client and server run simultaneously on the same host. The client communicates with the server using a REST API, which enables the client to also communicate with a remote server instance.

### Docker Client
[Back to Top](#docker)

A command-line application named `docker` that provides us wiht a command line interface (CLI) to interact with the Docker server. The `docker` command uses the Docker REST API to send instructions to either a local or remote server and functions as the primary interface we use to manage our containers.

### Docker Server
[Back to Top](#docker)

A daemon named `dockerd`. The `dockerd` daemon responds to requests from the client via the Docker REST API and can interact with other daemons. The Docker server is also responsible for tracking the lifecycle of our containers.

### Docker Objects
[Back to Top](#docker)

There are several objects that you'll create and configure to support your container deployments. These include networks, storage volumes, plugins, and other service objects.

### Docker Hub
[Back to Top](#docker)

A Software as a Service Docker container registry. Docker registries are repositories that we use to store and distribute the container images we create. Docker Hub is the default public registry Docker uses for image management.

You can create and use a private Docker registry or use one of the many cloud provider options available. For example, you can use Azure Container Registry to store container images to use in several Azure cotnainer-enabled services.

### Software
[Back to Top](#docker)

The software packaged into a container isn't limited to the applications our developers build. When we talk about software, we refer to application code, system packages, binaries, libraries, configuration files, and the operating system running in the container.

For example, assume we're developing an order tracking portal that our company's various outlets will use. We need to look at the complete stack of software that will run our web application. The application we're building is a .NET Core MVC app, and we plan to deploy the application using Nginx as a reverse proxy server on Ubuntu Linux. All of these software components form part of the container image.

### Images
[Back to Top](#docker)

A portable package that contains software. It's this image that, when run, becomes our container. The container is the in-memory instance of an image.

A container image is immutable. Once you've built an image, the image can't be changed. The only way to change an image is to create a new image. This feature is our guarantee that the image we use in production is the same image used in development and QA.

### Tags
[Back to Top](#docker)

A text string that is used to version an image. When building an image, we name and optionally tag the image using the `-t` command flag, with the convention *{name}:{tag}*. An image is labeled with the `latest` tag if you don't specify a tag (i.e. - *temp-ubuntu:latest*).

A single image can have multiple tags assigned to it. By convention, the most recnet version of an image is assigned the *latest* tag and a tag that describes the image version number. When you release a new versino of an image, you can reassign the latest tag to reference the new image.

Here is another example. Suppose you want to use the .NET Core samples Docker images. Here we have four platform versions that we can choose from:

* `mcr.microsoft.com/dotnet/core/samples:dotnetapp`
* `mcr.microsoft.com/dotnet/core/samples:aspnetapp`
* `mcr.microsoft.com/dotnet/core/samples:wcfservice`
* `mcr.microsoft.com/dotnet/core/samples:wcfclient`

### Host OS
[Back to Top](#docker)

![host-os](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/3-container-scratch-host-os.svg)

The host OS is the OS on which the Docker engine runs. Docker containers running on Linux share the host OS kernel and don't require a container OS as long as the binary can access the OS kernel directly.

### Container OS
[Back to Top](#docker)

![container-os](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/3-container-ubuntu-host-os.svg)

The container OS is the OS that is part of the packaged image. We have the flexibility to include different versions of Linux or Windows OSs in a container. This flexibility allows us to access specific OS features or install additional software our applications may use.

> Windows containers need a container OS. The container depends on the OS kernel to manage services such as the file system, network management, process scheduling, and memory management.

The container OS is isolated from the host OS and is the environment in which we deploy and run our application. Combined with the image's immutability, this isolation means the environment for our application running in development is the same as in production.

### Stackable Unification File System
[Back to Top](#docker)

![unionfs-diagram](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/3-unionfs-diagram.svg)

`Unionfs` is used to create Docker images. `Unionfs` is a filesystem that allows you to stack several directories, called branches, in such a way that it appears as if the content is merged. However, the content is physically kept separate. `Unionfs` allows you to add and remove branches as you build out your file system.

Assume we're building an image for our web application from earlier. We'll layer the Ubuntu distribution as a base image on top of the boot file system. Next we'll install Nginx and our web app. We're effectively layering Nginx and the web app on top of the original Ubuntu image.

A final writeable layer is created once the container is run from the image. This layer however, does not persist when the container is destroyed.

### Base Image
[Back to Top](#docker)

An image that uses the Docker `scratch` image. The `scratch` image is an empty container that doesn't create a filesystem layer. This image assumes that the application you're going to run can directly use the host OS kernel.

### Parent Image
[Back to Top](#docker)

A container image from which you create images.

Instead of creating an image from `scratch` and then installing Ubuntu, we'll rather use an image already based on Ubuntu. We can even use an image that already has Nginx installed. A parent image usually includes a container OS.

### Base vs. Parent Images
[Back to Top](#docker)

Both image types allow us to create a reusable image. However, base images allow us more control over the contents of the final image. Recall that an image is immutable, you can only add to an image and not subtract.

### Dockerfile
[Back to Top](#docker)

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

Another [Dockerfile](./assets/docker/Dockerfile) example.

## Image Management
[Back to Top](#docker)

Docker images are large files that get stored on your PC and we need tools to manage these files.

The Docker CLI allows us to manage images by building, listing, removing, and running them. We manage Docker images by using the `docker` client. The client doesn't execute the commands directly and sends all queries to the `dockerd` daemon.

### Build
[Back to Top](#docker)

Use the `docker build` command to build Docker images. Assume the [Dockerfile](#dockerfile) definition linked to build an image.

```bash
docker build -t temp-ubuntu .
```

Output generated from the build:

```
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

Notice that a number of commands are executed to install software and manage configuration. For example, in step 2, we run the `apt -y update` and `apt install -y` commands to update the OS. These commands execute in a running container that is created for that step. Once the command has run, the intermediate container is removed. The underlying cached image is kept on the build host and not automatically deleted. This optimization ensures that later builds reuse these images to speed up build times.

### List
[Back to Top](#docker)

Docker automatically configures a local image registry on your machine. You can view the images in this registry with:

``` bash
docker images
```

REPOSITORY | TAG | IMAGE ID | CREATED | SIZE
-----------|-----|----------|---------|-----
tmp-ubuntu | latest | f89469694960 | 14 minutes ago | 1.69GB
tmp-ubuntu | version-1.0 | f89469694960 | 14 minutes ago | 1.69GB
ubuntu | 18.04 | a2a15febcdf3 | 5 weeks ago | 64.2MB

Notice how the image is listed with its *Name*, *Tag*, and an *Image ID*. Recall that we can apply multilpe labels to an image. Here is such an example. Even though the image names are different, we can see the IDs are the same.

The image ID is a useful way to identify and manage images where the name or tag of an image might be ambiguous.

### Remove
[Back to Top](#docker)

You can remove an image from the local docker registry with the `docker rmi` command. Specify the *name* or *ID* of the image to remove:

```bash
docker rmi temp-ubuntu:version-1.0
```

You can't remove an image if the image is still in use by a container. The `docker rmi` command returns an error message, which clists the container relying on the image.

## Container Management

![container-lifecycle](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/4-docker-container-lifecycle-2.png)

A Docker container has a lifecycle that you can manage and track the state of the container.

To place a container in the run state, use the run command. You can also restart a container that is already running. When restarting a container, the container receives a termination signal to enable any running processes to shut down gracefully before the container's kernel is terminated.

A container is considered in a running state until it's either paused, stopped, or killed. A container, however, may also exit from the run state by itself. A container can self-exit when the running process completes, or if the process goes into a fault state.

To pause a running container, use the pause command. This command suspends all processes in the container.

To stop a running container, use the stop command. The stop command enables the working process to shut down gracefully by sending it a termination signal. The container's kernel terminates after the process shuts down.

To send a kill signal if you need to terminate the container, use the kill command. The running process doesn't capture the kill signal, only the container's kernel. This command will forcefully terminate the working process in the container.

Lastly, to remove containers that are in a stopped state, use the remove command. After removing a container, all data stored in the container gets destroyed.

### View
[Back to Top](#docker)

To list running containers, run the `docker ps` command. To see all containers in all states, pass the `-a` argument.

```bash
docker ps -a
```

CONTAINER ID | IMAGE | COMMAND | CREATED | STATUS | PORTS | NAMES
-------------|-------|---------|---------|--------|-------|------
d93d40cc1ce9 | tmp-ubuntu:latest | "dotnet website.dll …" | 6 seconds ago | Up 5 seconds | 8080/tcp | happy_wilbur
33a6cf71f7c1 | tmp-ubuntu:latest | "dotnet website.dll …" | 2 hours ago | Exited (0) 9 seconds ago | | adoring_borg

There are three items to review in the above output:

* The image name listed in the **IMAGE** column. Notice how you're allowed to create more than one container from the same image. This feature is a powerful management feature that you use to enable scaling in your solutions.
* The status of the container listed in the **STATUS** column. In the above output, you have one container that is running, and one container that has exited. The container's status usually is your first indicator of the health of the container.
* The name of the container listed in the **NAMES** column. Apart from the container ID in the first column, containers will also receive a name. In this example, you didn't explicitly provide a name for each container, and as a result, Docker gave the container a random name. To give a container an explicit name using the `--name` flage, use the run command.
    * Names enable you to run multiple container instances of the same image. Container names are unique, which means if you specify a name, that name can't be reused to create a new container. The only way to reuse a specific name is to remove the previous container.

### Run
[Back to Top](#docker)

To start a container, run the `docker run` command. You only need to specify the image to run with its name or ID to launch the container from the image. A container launched in this manner provides an interactive experience.

To run the container with our website int he background, add the `-d` flag.

```bash
docker run -d tmp-ubuntu
```

The command, in this case, only returns the ID of the new container.

After an image is specified to run, Docker finds the image, loads container from the image, and executes the command specified as the entry point. It's at this point that the container is available for management.

### Pause
[Back to Top](#docker)

To pause a container:

```bash
docker pause happy_wilbur
```

Pausing a container will suspend all processes. This command enables the container to continue processes at a later stage.

To unsuspend all processes in a specified container:

```bash
docker unpause happy_wilbur
```

### Restart
[Back to Top](#docker)

To restart containers:

```bash
docker restart happy_wilbur
```

The container receives a stop command, followed by a start command. If the container doesn't respond to the stop command, thena  kill signal is sent.

### Stop
[Back to Top](#docker)

To stop a running container:

```bash
docker stop happy_wilbur
```

The stop command sends a termination signal to the container and the process running in the container.

### Remove
[Back to Top](#docker)

To remove a container:

```bash
docker rm happy_wilbur
```

After you remove the container, all data in the container is destroyed. It's essential to always consider containers as temporary when thinking about storing data.

### Storage Configuration
[Back to Top](#docker)

Always consider containers as temporary when the app in a container needs to store data.

Assume your app creates a log file in a subfolder to the root of the app; that is, directly to the file system of the container. When your app writes data ot the log file, the system writes the data to the writable container layer.

Even though this approach works, it has several drawbacks:

* Container storage is temporary
    * Your log file won't persist between container instances. Assume the container is stopped and removed. When you launch a new container instance, the new instance bases itself on the image specified, and all your previous data will be missing. All data in a container is destroyed with the container when you remove it.
* Container storage is coupled to the underlying host machine
    * Accessing or moving the log file from the container is difficult to do as the container is coupled to the underlying host machine. You'll have to connect to the container instance to access the file.
* Container storage drives are less performant
    * Containers implement a storage driver to allow your apps to write data. This driver introduces an extra abstraction to communicate with the host OS kernel and is less performant than writing directly to a host filesystem.

Containers can make use of two options to persist data. The first option is to make use of *volumes*, and the second is *bind mounts*.

### Volume
[Back to Top](#docker)

A volume is stored on the host filesystem at a specific folder location. Choose a folder where you know the data isn't going to be modified by non-Docker processes.

Docker creates and manages the new volume by running the `docker volume create` command. This command can form part of our Dockerfile definition, which means that you can create volumes as part of the container creation process. Docker will create the volume if it doesn't exist when you try to mount the volume into a container the first time.

Volumes are stored within directories on the host filesystem. Docker will mount and manage the volumes in the container. After mounting, these volumes are isolated from the host machine.

Multiple containers can simultaneously use the same volumes. Volumes also don't get removed automatically when a container stops using a volume.

### Bind Mount
[Back to Top](#docker)

A bind mount is conceptually the same as a volume, however, instead of using a specific folder, you can mount any file or folder on the host. You're also expecting the host can change the contents of these mounts. Just like volumes, the bind mount is created if you mount it, and doesn't yet exist on the host.

Bind mounts have limited functionality compared to volumes, and even though they're more performant, they depend on the host having a specific folder structure in place.

Volumes are considered the preferred data storage strategy to use with containers.

### Network Configuration
[Back to Top](#docker)

The default docker network configuration allows for the isolation of containers on the Docker host. This feature enables you to build and configure apps that can communicate securely with each other.

Docker provides three pre-configured network configurations:

* Bridge
* Host
* none

You choose which of these network configurations to apply to your container depending on its network requirements.

### Bridge Network
[Back to Top](#docker)

The default configuration applied to containers when launched without specifying any other network configuration. This network is an internal, private network used by the container and isolates the container network from the Docker host network.

Each container in the bridge network is assigned an IP address and subnet mask with the hostname defaulting to the container name. Containers connected to the default bridge network are allowed to access other bridge connected containers by IP address. The bridge network doesn't allow communication between containers using hostnames.

By default, Docker doesn't publish any container ports. To enable port mapping between the container ports and the Docker host ports, use the Docker port `--publish` flag.

The public flag effectively configures a firewall rule that maps the ports.

The container resource is accessible to clients browsing to port 80. You'll have to map port 80 from the container to an available port on the host. You have port 8080 open on the host, which enables you to set the flag like this:

```bash
--publish 8080:80
```

Any client browsing to the Docker host IP and port 8080 can access the container resource.

### Host Network
[Back to Top](#docker)

Enables you to run the container on the host network directly. This configuration effectively removes the isolation between the host and the container at a network level.

Let's assume you decide to change the networking configuration to the host network option. Your container resource is still accessible using the host IP. You can now use the well known port 80 instead of a mapped port.

Keep in mind that the container can use only ports not already used by the host.

### None Network
[Back to Top](#docker)

To disable networking for containers, use the none network option.

### OS Considerations
[Back to Top](#docker)

There are differences between desktop operating systems for the Docker network configuration options. For example, the *Docker0* network interface isnt' available on macOS when using the bridge network, and using the host network configuration isn't supported for both Windows and macOS desktops.

These differences might affect the way your developers configure their workflow to manage container development.

## Cheatsheet
[Back to Top](#docker)

```bash
# view images in the local Docker image registry
docker images

# remove an image from the local Docker image registry
docker rmi <tag>:<version>

# place a container in the run state
docker run -d tmp-ubuntu

# run a .NET Web API container on port 5000
docker run -it --rm -p 5000:80 {image}

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

# login to Docker hub
docker login

# re-tag Docker image under username
docker tag {image} {username}/{image}

#push image to Docker Hub
docker push {username}/{image}
```

### Docker Compose
[Back to Top](#docker)

```bash
# build from docker-compose.yml
docker-compose build

# run orchestrated images
docker-compose up
```

### Docker Hub
[Back to Top](#docker)

```bash
docker login

docker tag {image} {username}/{image}

docker push {username}/{image}
```