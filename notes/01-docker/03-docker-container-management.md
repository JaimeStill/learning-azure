# Container Management

* [View](#view)
* [Run](#run)
* [Pause](#pause)
* [Restart](#restart)
* [Stop](#stop)
* [Remove](#remove)
* [Storage Configuration](#storage-configuration)
    * [Volume](#volume)
    * [Bind Mount](#bind-mount)
* [Network Configuration](#network-configuration)
    * [Bridge Network](#bridge-network)
    * [Host Network](#host-network)
    * [None Network](#none-network)
    * [OS Considerations](#os-considerations)

![container-lifecycle](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/media/4-docker-container-lifecycle-2.png)

A Docker container has a lifecycle that you can manage and track the state of the container.

To place a container in the run state, use the run command. You can also restart a container that is already running. When restarting a container, the container receives a termination signal to enable any running processes to shut down gracefully before the container's kernel is terminated.

A container is considered in a running state until it's either paused, stopped, or killed. A container, however, may also exit from the run state by itself. A container can self-exit when the running process completes, or if the process goes into a fault state.

To pause a running container, use the pause command. This command suspends all processes in the container.

To stop a running container, use the stop command. The stop command enables the working process to shut down gracefully by sending it a termination signal. The container's kernel terminates after the process shuts down.

To send a kill signal if you need to terminate the container, use the kill command. The running process doesn't capture the kill signal, only the container's kernel. This command will forcefully terminate the working process in the container.

Lastly, to remove containers that are in a stopped state, use the remove command. After removing a container, all data stored in the container gets destroyed.

## View
[Back to Top](#container-management)

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

## Run
[Back to Top](#container-management)

To start a container, run the `docker run` command. You only need to specify the image to run with its name or ID to launch the container from the image. A container launched in this manner provides an interactive experience.

To run the container with our website int he background, add the `-d` flag.

```bash
docker run -d tmp-ubuntu
```

The command, in this case, only returns the ID of the new container.

After an image is specified to run, Docker finds the image, loads container from the image, and executes the command specified as the entry point. It's at this point that the container is available for management.

## Pause
[Back to Top](#container-management)

To pause a container:

```bash
docker pause happy_wilbur
```

Pausing a container will suspend all processes. This command enables the container to continue processes at a later stage.

To unsuspend all processes in a specified container:

```bash
docker unpause happy_wilbur
```

## Restart
[Back to Top](#container-management)

To restart containers:

```bash
docker restart happy_wilbur
```

The container receives a stop command, followed by a start command. If the container doesn't respond to the stop command, thena  kill signal is sent.

## Stop
[Back to Top](#container-management)

To stop a running container:

```bash
docker stop happy_wilbur
```

The stop command sends a termination signal to the container and the process running in the container.

## Remove
[Back to Top](#container-management)

To remove a container:

```bash
docker rm happy_wilbur
```

After you remove the container, all data in the container is destroyed. It's essential to always consider containers as temporary when thinking about storing data.

## Storage Configuration
[Back to Top](#container-management)

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
[Back to Top](#container-management)

A volume is stored on the host filesystem at a specific folder location. Choose a folder where you know the data isn't going to be modified by non-Docker processes.

Docker creates and manages the new volume by running the `docker volume create` command. This command can form part of our Dockerfile definition, which means that you can create volumes as part of the container creation process. Docker will create the volume if it doesn't exist when you try to mount the volume into a container the first time.

Volumes are stored within directories on the host filesystem. Docker will mount and manage the volumes in the container. After mounting, these volumes are isolated from the host machine.

Multiple containers can simultaneously use the same volumes. Volumes also don't get removed automatically when a container stops using a volume.

### Bind Mount
[Back to Top](#container-management)

A bind mount is conceptually the same as a volume, however, instead of using a specific folder, you can mount any file or folder on the host. You're also expecting the host can change the contents of these mounts. Just like volumes, the bind mount is created if you mount it, and doesn't yet exist on the host.

Bind mounts have limited functionality compared to volumes, and even though they're more performant, they depend on the host having a specific folder structure in place.

Volumes are considered the preferred data storage strategy to use with containers.

## Network Configuration
[Back to Top](#container-management)

The default docker network configuration allows for the isolation of containers on the Docker host. This feature enables you to build and configure apps that can communicate securely with each other.

Docker provides three pre-configured network configurations:

* Bridge
* Host
* none

You choose which of these network configurations to apply to your container depending on its network requirements.

### Bridge Network
[Back to Top](#container-management)

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
[Back to Top](#container-management)

Enables you to run the container on the host network directly. This configuration effectively removes the isolation between the host and the container at a network level.

Let's assume you decide to change the networking configuration to the host network option. Your container resource is still accessible using the host IP. You can now use the well known port 80 instead of a mapped port.

Keep in mind that the container can use only ports not already used by the host.

### None Network
[Back to Top](#container-management)

To disable networking for containers, use the none network option.

### OS Considerations
[Back to Top](#container-management)

There are differences between desktop operating systems for the Docker network configuration options. For example, the *Docker0* network interface isnt' available on macOS when using the bridge network, and using the host network configuration isn't supported for both Windows and macOS desktops.

These differences might affect the way your developers configure their workflow to manage container development.