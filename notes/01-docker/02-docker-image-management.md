# Image Management

Docker images are large files that get stored on your PC and we need tools to manage these files.

The Docker CLI allows us to manage images by building, listing, removing, and running them. We manage Docker images by using the `docker` client. The client doesn't execute the commands directly and sends all queries to the `dockerd` daemon.

### Build
[Back to Top](#image-management)

Use the `docker build` command to build Docker images. Assume the [Dockerfile](./01-docker-architecture.md#dockerfile) definition linked to build an image.

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

## List
[Back to Top](#image-management)

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

## Remove
[Back to Top](#image-management)

You can remove an image from the local docker registry with the `docker rmi` command. Specify the *name* or *ID* of the image to remove:

```bash
docker rmi temp-ubuntu:version-1.0
```

You can't remove an image if the image is still in use by a container. The `docker rmi` command returns an error message, which clists the container relying on the image.