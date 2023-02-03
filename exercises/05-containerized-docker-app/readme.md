# Build a Containerized Web App with Docker

## Retrieve an Existing Docker Image and Deploy It Locally

A good starting point for building and running your own Docker images is to take an existing image from Docker Hub and run it locally on your computer.

### Pull and Run a Sample App from Docker Hub

1. Pull the `mcr.microsoft.com/dotnet/samples:aspnetapp` image:

    ```bash
    docker pull mcr.microsoft.com/dotnet/samples:aspnetapp
    ```

2. Verify the image has been stored locally:

    ```bash
    docker image ls
    ```

    REPOSITORY | TAG | IMAGE ID | CREATED | SIZE
    -----------|-----|----------|---------|-----
    mcr.microsoft.com/dotnet/samples | aspnetapp | be79247d6514 | 9 days ago | 84.5MB

3. Run the image in the background and map container port 80 to local port 8080:

    ```bash
    docker run -d -p 8080:80 mcr.microsoft.com/dotnet/samples:aspnetapp
    ```

4. Open the browser to the web app URL:

    ![image](https://user-images.githubusercontent.com/14102723/216357939-47fb904f-3dea-44ba-85e4-4038243ce8c7.png)

### Examine the Container in the Local Docker Registry

1. View the running containers in the local registry:

    ```bash
    docker ps
    ```

    CONTAINER ID | IMAGE | COMMAND | CREATED | STATUS | PORTS | NAMES
    -------------|-------|---------|---------|--------|-------|------
    ed601b23f1d2 | mcr.microsoft.com/dotnet/samples:aspnetapp | "./aspnetapp" | 2 minutes ago | Up 2 minutes | 0.0.0.0:8080->80/tcp, :::8080->80/tcp | clever_black

    The **COMMAND** field shows the container started by running the command `./aspnetapp`. This command invokes the .NET Core runtime to start the app. The *PORTS* field indicates that port 80 in the image was mapped to port 8080 on your computer. The *STATUS* field shows the application is still running. Make note of the container's *NAME*.

2. Stop the docker container:

    ```bash
    docker container stop clever_black
    ```

3. Verify the container is no longer running:

    ```bash
    # -a indicages that the command shows all containers
    docker ps -a
    ```

    CONTAINER ID | IMAGE | COMMAND | CREATED | STATUS | PORTS | NAMES
    -------------|-------|---------|---------|--------|-------|------
    ed601b23f1d2 | mcr.microsoft.com/dotnet/samples:aspnetapp | "./aspnetapp" | 6 minutes ago | Exited (0) 23 seconds ago | | clever_black

### Remove the Container and Image from the Local Registry

1. Remove the container:

    ```bash
    docker container rm clever_black
    ```

2. Verify the container has been removed:

    ```bash
    docker ps -a
    ```

3. List the images currently available on your computer:

    ```bash
    docker image ls
    ```

    The samples repository image is still included in the local image registry.

4. Remove the image from the registry:

    ```bash
    docker image rm mcr.microsoft.com/dotnet/samples:aspnetapp
    ```

    Output:

    ```
    Untagged: mcr.microsoft.com/dotnet/samples:aspnetapp
    Untagged: mcr.microsoft.com/dotnet/samples@sha256:e7e04d4ab9da0c3da16ef088baa97e88bb655aa29fe27cb8c6f0564dd42538b5
    Deleted: sha256:be79247d6514faea1d25a1e648d66e9c419338632b6974ae6197d5a269298b8f
    Deleted: sha256:1ee6b5691b3b209d4d6c4a9d7dd14b87541684f371f7360e6cf5be259d6993f0
    Deleted: sha256:eb70a060bd4132e3e5acc22c3be3cdbdf35915fbb4f05a265ae4c9fffb89fa0b
    Deleted: sha256:0512ae47d6aaa5cd507d26b9817de26cdea28efdf41813f15d2066a6bb3b55f4
    Deleted: sha256:8e012198eea15b2554b07014081c85fec4967a1b9cc4b65bd9a4bce3ae1c0c88
    ```

5. Verify the image has been removed from the local image registry:

    ```bash
    docker image ls
    ```

## Customize a Docker Image to Run Your Own Web App

A [Dockerfile](https://docs.docker.com/engine/reference/builder/) contains the steps for building a custom Docker image.

In this exercise, you'll create a Dockerfile for an app that doesn't have one. Then, you'll build the image and run it locally.

### Create a Dockerfile for the Web App

1. Download the source code for the web app:

    ```bash
    git clone https://github.com/MicrosoftDocs/mslearn-hotel-reservation-system.git
    ```

    > The following steps were used to setup the src directory in this repository
    > 
    > ```bash
    > cd mslearn-hotel-reservation-system
    > rm -rf .git
    > cd ..
    > mv mslearn-hotel-reservation-system/ app/
    > ```

2. Change to the `src` directory:

    ```bash
    cd app/src/
    ```

3. Create and open a `Dockerfile`:

    ```bash
    touch Dockerfile
    code Dockerfile
    ```

4. Add the following to the Dockerfile:

    ```Dockerfile
    FROM mcr.microsoft.com/dotnet/core/sdk:2.2
    WORKDIR /src
    COPY ["/HotelReservationSystem/HotelReservationSystem.csproj", "HotelReservationSystem/"]
    COPY ["/HotelReservationSystemTypes/HotelReservationSystemTypes.csproj", "HotelReservationSystemTypes/"]
    RUN dotnet restore "HotelReservationSystem/HotelReservationSystem.csproj"
    ```

    This code has commands to fetch an image containing thet .NET Core SDK. The project files for the web app (`HotelReservationSystem.csproj`) and the library project (`HotelReservationSystemTypes.csproj`) are copied ot the `/src` folder in the container. The `dotnet restore` command downloads the dependencies required by these projects from NuGet.

5. Append the following code to the bottom of the Dockerfile:

    ```Dockerfile
    COPY . .
    WORKDIR "/src/HotelReservationSystem"
    RUN dotnet build "HotelReservationSystem.csproj" -c Release -o /app
    ```

    These commands copy the source code for the web app to the container, and then run the dotnet build command to build the app. The resulting DLLs are written to the `/app` folder in the container.

6. Append the following command at the bottom of the Dockerfile:

    ```Dockerfile
    RUN dotnet publish "HotelReservationSystem.csproj" -c Release -o /app
    ```

    The `dotnet publish` command copies the executables for the website to a new folder and removes any interim files. The files in this folder can then be deployed to a website.

7. Append the following commands to the bottom of the Dockerfile:

    ```Dockerfile
    EXPOSE 80
    WORKDIR /app
    ENTRYPOINT ["dotnet", "HotelReservationSystem.dll"]
    ```
    
    These commands:
    * Open port 80 in the container
    * Moves to the `/app` folder containing the published version of the web app
    * Specifies when the container runs it should execute the command `dotnet HotelReservationSystem.dll`

### Build and Deploy the Image Using the Dockerfile

1. Build the image for the sample app using the Dockerfile:

    ```bash
    docker build -t reservationsystem .
    ```

    This command builds the image and stores it locally. The image is given the name `reservationsystem`.

2. Verify the image has been created and stored in the local registry:

    ```bash
    docker image list
    ```

    REPOSITORY | TAG | IMAGE ID | CREATED | SIZE
    -----------|-----|----------|---------|-----
    reservationsystem | latest | 626a2779f54f | 39 seconds ago | 1.87GB

### Test the Web App

1. Run a container named `reservations` in the background using the `reservationsystem` image on port 8080:

    ```bash
    docker run -p 8080:80 -d --name reservations reservationsystem
    ```

2. Navigating to `http://localhost:8080/api/reservations/1` should deliver:

    ```json
    {
        "reservationID": 1,
        "customerID": "93048732",
        "hotelID": "Hotel 1810120949",
        "checkin": "2023-02-12T15:54:09.9477533+00:00",
        "checkout": "2023-04-05T07:58:48.1777554+00:00",
        "numberOfGuests": 5,
        "reservationComments": "G/W1S/yCI+z7DV4m5MIGoYi/6GoNzd94E7Mz6aQADbbyBUXHVxWshBuXFD0saPDWzyqSNqDC+Ug7gFbBNxeIutUxWfsWgyLRJmMgv3cIaiB/LnyYniS+KhcgZ7tHExrRlvqzzkLWY+ICyJGFmIzQdteMYNvZNkZ7E6ULgWmT+44YVgalYxwuuDWZ"
    }
    ```

3. Stop the `reservations` container:

    ```bash
    docker container stop reservations
    ```

4. Delete the `reservations` container from the local registry:

    ```bash
    docker rm reservations
    ```

    > Leave the `reservationsystem` image in the local registry, as it will be used for upcoming sections.

## Deploy a Docker Image to an Azure Container Instance

Azure Container Instance enables you to run a Docker image in Azure.

In this exercise, you'll learn how to rebuild the image for the web app and upload it to Azure Container Registry. You'll use the Azure Container Instance service to run the image.

### Create a Container Registry

1. Install the Azure CLI:

    ```bash
    curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
    ```

2. Login to the Azure CLI:

    ```bash
    az login --use-device-code
    ```

3. Create an Azure Resource Group:

    ```bash
    az group create --name rg-reservations --location eastus
    ```

4. Verify your desired ACR registry name is available using the [Registries - Check Name Availability](https://learn.microsoft.com/en-us/rest/api/containerregistry/registries/check-name-availability) tool.

5. Create an Azure Container Registry:

    ```bash
    # registry = jpslearnacr
    az acr create --name <registry> --resource-group rg-reservations --sku standard --admin-enabled true
    ```

### Upload the Image to Azure Container Registry 

1. Look up the access keys for the registry:

    ```bash
    az acr credential show --name jpslearnacr --resource-group rg-reservations
    ```

    Output:

    ```json
    {
      "passwords": [
        {
          "name": "password",
          "value": "{password}"
        },
        {
          "name": "password2",
          "value": "{password}"
        }
      ],
      "username": "jpslearnacr"
    }
    ```

2. Login to your container registry with the credentials retrieved in the preceding step:

    ```bash
    docker login jpslearnacr.azurecr.io
    ```

3. Create an alias that specifies the repository and tagf or your image. Must be in the form `<login_server>/<image_name>:<tag>`:

    ```bash
    docker tag reservationsystem jpslearnacr.azurecr.io/reservationsystem:v2
    ```

4. Upload the image to the Azure Container Registry:

    ```bash
    docker push jpslearnacr.azurecr.io/reservationsystem:v2
    ```

5. Verify the image has been uploaded correctly:

    List the repositories in the registry:

    ```bash
    az acr repository list --name jpslearnacr --resource-group rg-reservations
    ```

    Output:

    ```json
    [
        "reservationsystem"
    ]
    ```

    List images in the registry:

    ```bash
    az acr repository show --repository reservationsystem --name jpslearnacr --resource-group rg-reservations
    ```

    Output:

    ```json
    {
      "changeableAttributes": {
        "deleteEnabled": true,
        "listEnabled": true,
        "readEnabled": true,
        "teleportEnabled": false,
        "writeEnabled": true
      },
      "createdTime": "2023-02-02T16:29:01.043874Z",
      "imageName": "reservationsystem",
      "lastUpdateTime": "2023-02-02T16:29:01.1166526Z",
      "manifestCount": 1,
      "registry": "{registry}.azurecr.io",
      "tagCount": 1
    }
    ```

### Run an Image Using Azure Container Instance

1. You create a container instance and start the image running with the `az container create` command:

    ```bash
    az container create --resource-group rg-reservations --name reservations --image jpslearnacr.azurecr.io/reservationsystem:v2 --dns-name-label reservations --registry-username <username> --registry-password <password>
    ```

2. Azure hosts the instance with a domain name based on the DNS label you specified. You can find the fully qualified domain name of the instance by querying the IP address of the instance:

    ```bash
    az container show --resource-group rg-reservations --name reservations --query ipAddress.fqdn
    ```

    Output: `reservations.eastus.azurecontainer.io`

3. Test the API endpoint in the browser with the URL http://reservations.eastus.azurecontainer.io/api/reservations/1

    ```json
    {
        "reservationID": 1,
        "customerID": "252999531",
        "hotelID": "Hotel 826868717",
        "checkin": "2023-02-12T17:10:44.7613491+00:00",
        "checkout": "2023-02-15T17:40:54.5143533+00:00",
        "numberOfGuests": 4,
        "reservationComments": "2bOHJ5uoe81f+6njjkSzG+vWiB0lUNuGBcmpEGza4e1mDF6Am0d0wV7B4JWiBXvlIH3lW5oWUgESbgqDnAuNCclvTTlb2iUtKHUqzDvexsN9CrmOTtozaFckpXjgW4N9Ne6V1qSmzxP72qnnoIVAB3m/EMkgfd16ij7nlDXtX9C0Ry8+ylc2VgBxn+/mWgu51QLZpOAGt+xhv0UoUZ5RFJwQ1HeJ+DFDSNT189EVn6/NsuAb8NF7jVpXCIKkj1ZUJTXo+XdY42pbFyzNHEIngNoY94rK0WKZO58XPK0mWprMnm3DZaN6U8I/2TiX/rwC8hluEsPE84fFnUhOuOA6QVQbKzdM/gvh36zeCnSYgHHhhe2rWQicgsoOx9fdXVqFaacnqN6THjYec12h5Gtp9KpSCKfsQRCTBxS+h4LrM7WM2oEGBslAMaw+GrdgfLuRE/9OIyHZraf6l6Evn/IiaftBH+l/syWE8rrMpWB3SToarSQX8yTxTaUy9igAx52GNGtyz53+SesJAey/WAPne52JTBBWqBXM+ujbEGZnJgW39Z8b5mVOvTgq2FmbciUbiuOzP1X8KeXl/qf0YRhEhluvUKaTk7FVGNn12wNHc1cd3mrqXY/Z25fOlKXudA+nbKEAKAScqpmCemJfsfKn197LxBuJ4c5NCKzkanwDt5MO1JZQKn18cripLGlpWU9wA333dvAZl/KU7K4NwfV9PsDEUhHPSN87aNLPDdHWuVpF1hLreHcPNZd+qd3ZFD/H994O4f8K26OpEOmBopbfc+zEHo/KObKIMttKyrI1MwTPt2BdLlgLSfWN6CzWmP0BfMr+GtK3nRQ+lucOqKIiCj6zgQulqEeh7MBf1PYm+zbirPazsw=="
    }
    ```

## Clean Up Resources

1. Remove Docker images from the local registry:

    ```bash
    docker image rm reservationsystem
    docker image rm jpslearnacr.azurecr.io/reservationsystem:v2
    ```

2. List the Resource Groups in the Azure CLI:

    ```bash
    az group list -o table
    ```

    Output:

    NAME | LOCATION | STATUS
    -----|----------|-------
    **rg-reservations** | eastus | Succeeded

3. Delete the **rg-reservations** resource group:

    ```bash
    az group delete -n rg-reservations -y
    ```
