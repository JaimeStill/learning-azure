# Docker Cheatsheet

**Docker Container Lifecycle**  

![docker-container-lifecycle](https://user-images.githubusercontent.com/14102723/211899857-e41d4c91-ed09-40c7-826c-ee8249fe408c.png)

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

## Docker Compose

```bash
# build from docker-compose.yml
docker-compose build

# run orchestrated images
docker-compose up
```

## Docker Hub

```bash
docker login

docker tag {image} {username}/{image}

docker push {username}/{image}
```

## Dockerfile
[Back to Top](#learning-containers)

A [`Dockerfile`](./assets/Dockerfile) is a text file that contains the instructions used to build and run a Docker image. The following aspects of the image are defined:

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
[Back to Top](#learning-containers)

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