# Deploy to AKS

https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/

## Create an AKS Cluster
[Back to Top](#deploy-to-aks)

1. Sign in to Azure in WSL

    ```bash
    az login
    ```

2. Create variables for the configuration values you'll reuse throughout the exercises.

    ```bash
    export RESOURCE_GROUP=rg-contoso-video
    export CLUSTER_NAME=aks-contoso-video
    export LOCATION=eastus
    ```

3. Run the `az group create` command to create a resource group. You'll deploy all resources into this new resource group.

    ```bash
    az group create --name=$RESOURCE_GROUP --location=$LOCATION
    ```

4. Run the `az aks create` command to creae an AKS cluster.

    **Saved as [create-aks.sh](./assets/create-aks.sh)**

    ```bash
    az aks create \
        --resource-group $RESOURCE_GROUP \
        --name $CLUSTER_NAME \
        --node-count 2\
        --enable-addons https_application_routing \
        --generate-ssh-keys \
        --node-vm-size Standard_B2s \
        --network-plugin azure \
        --windows-admin-username localadmin
    ```

    The command creates a new AKS cluster named `aks-contoso-video` with the `rg-contoso-video` resource group. The cluster will have two nodes defined by the `--node-count` parameter. We're using only two nodes in this exercise for cost considerations. The `--node-vm-size` parameter configures the cluster nodes as *Standard_B2s*-sized VMs. The HTTP application routing add-on is enabled via the `--enable-addons` flag. These nodes will be part of **System** mode.

    The `--windows-admin-username` parameter is used to setup administrator credentials for Windows containers, and prompts the user to set a password at the command line. the password as to meet [Windows Server password requirements](https://learn.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/password-must-meet-complexity-requirements#reference).

5. Run the `az aks nodepool add` command to add another node pool that uses the Windows operating system.

    **Saved as [add-aks-nodepool.sh](./assets/add-aks-nodepool.sh)**

    ```bash
    az aks nodepool add \
        --resource-group $RESOURCE_GROUP \
        --cluster-name $CLUSTER_NAME \
        --name uspool \
        --node-count 2 \
        --node-vm-size Standard_B2s \
        --os-type Windows
    ```

    The command adds a new node pool (**User** mode) to an existing AKS cluster. This new node pool can be used to host applications and workloads, instead of using the **System** node pool, which was created using `az aks create` above.

    The `--os-type` parameter is used to specify the operating system of the node pool. If not specified, the command will use Linux as the operating system for the nodes.

### Link with `kubectl`
[Back to Top](#deploy-to-aks)

1. Link your Kubernetes cluster with `kubectl` by running the following command:

    ```bash
    az aks get-credentials --name $CLUSTER_NAME --resource-group $RESOURCE_GROUP

    # copy from Windows users directory to wsl user directory
    cp /mnt/c/Users/{user}/.kube/config ~/.kube/config
    ```

    This command will add an entry to your `~/.kube/config` file (see [kube.config](./assets/kube.config) for a sanitized example), which holds all the information to access your clusters. Kubectl enables you to manage multiple clusters from a single command-line interface.

2. Run the `kubectl get nodes` command to check that you can connect to your cluster, and confirm its configuration.

    ```bash
    kubectl get nodes
    ```

    Output:

    NAME | STATUS | ROLES | AGE | VERSION
    -----|--------|-------|-----|--------
    aks-nodepool1-16797674-vmss000000 | Ready | agent | 39m | v1.24.6
    aks-nodepool1-16797674-vmss000001 | Ready | agent | 39m | v1.24.6
    aksuspool000000 | Ready | agent | 29m | v1.24.6
    aksuspool000001 | Ready | agent | 30m | v1.24.6

## Create a Deployment Manifest
[Back to Top](#deploy-to-aks)

You create a deployment manifest file to deploy your application. The manifest file allows you to define what type of resource you want to deploy and all the details associated with the workload.

Kubernetes groups containers into logical structures called pods, which have no intelligence. Deployments add the missing intelligence to create your application. Let's create a deployment file.

1. Login to the Azure CLI

    ```bash
    az login
    ```

2. Create a manifest file ofr the Kubernetes deployment called [`deployment.yaml`](./assets/deployment.yaml):

    ```bash
    touch ./deployment.yaml
    ```

3. Open the file in VS code:

    ```bash
    code ./deployment.yaml
    ```

4. Add the following code section of YAML:

    ```yaml
    apiVersion: apps/v1 # the API resource where this workload resides
    kind: Deployment # the kind of workload we're creating
    metadata:
        name: contoso-website # this will be the name of the deployment
    ```

    In this code, you added the first two keys to tell Kubernetes the `apiVersion` and `kind` of manifest you're creating. The `name` is the name of the deployment. You'll use it to identify and query the deployment information when you use `kubectl`.

5. A deployment wraps a pod. You make use of a template definition to define the pod information within the manifest file. The template is placed in the manifest file under the deployment specification section.

    Update the [`deployment.yaml`](./assets/deployment.yaml) file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      template: # this is the teamplte of the pod inside the deployment
        metadata: # metadata for the pod
          labels:
            app: contoso-website
    ```

    Pods don't have given names when they're created inside deployments. The pod's name will be the deployment's name iwht a random ID added to the end.

    Notice the use of the `labels` key. You add the `labels` key to allow deployments to find and group pods.

6. A pod wraps one or more containers. All pods have a specification section that allows you to define the containers inside that pod.

    Update the [`deployment.yaml`](./assets/deployment.yaml) file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      template:
        metadata:
          labels:
            app: contoso-website
        spec:
          containers: # here we define all containers
            - name: contoso-website
    ```

    the `containers` key is an array of container specifications because a pod can have one or more containers. The specification defines an `image`, a `name`, `resources`, `ports`, and other important information about the container.

    All running pods will follow the name `contoso-website-<UUID>`, where UUID is a generated ID to identify all resources uniquely.

7. It's a good practice to define a minimum and maximum amount of resources that the app is allowed to use from the cluster. You use the `resources` key to specify this information.

    Update the `deployment.yaml` file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      template:
        metadata:
          labels:
            app: contoso-website
        spec:
          containers:
            - image: mcr.microsoft.com/mslearn/samples/contoso-website
              name: contoso-website
              resources:
                requests: # minimum amount of resources requested
                  cpu: 100m
                  memory: 128Mi
                limits: # maximum amount of resources requested
                  cpu: 250m
                  memory: 256Mi
    ```

    Notice how the resoruce section allows you to specify the minimum resource amount as a request and the maximum resource amount as a limit.

8. The last step is to define the ports this container will expose externally through the `ports` key. The `ports` key is an array of objects, which means that a container in a pod can expose multiple ports with multiple names.

    Update the [`deployment.yaml`](./assets/deployment.yaml) file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      template:
        metadata:
          labels:
            app: contoso-website
        spec:
          nodeSelector:
            kubernetes.io/os: linux
          containers:
            - image: mcr.microsoft.com/mslearn/samples/contoso-website
              name: contoso-website
              resources:
                requests:
                  cpu: 100m
                  memory: 128Mi
                limits:
                  cpu: 250m
                  memory: 256Mi
              ports:
                - containerPort: 80 # this container exposes port 80
                  name: http # we named the port "http" so we can refer to it later
    ```

    > In an AKS cluster which has multiple node pools (Linux and Windows), the deployment manifest file listed above defines a `nodeSelector` to tell your AKS cluster to run the sample application's pod on a node that can run Linux containers. Linux nodes can't run Windows containers and vice versa.

    Notice how you name the port by using the `name` key. Naming ports allows you to change the exposed port without changing files that reference that port.

9. Finally, add a selector section to define the workloads the deployment will manage. The `selector` key is placed inside the deployment specification section on the manifest file. Use the `matchLabels` key to list the labels fo rall the pods managed by the deployment.

    Update the [`deployment.yaml`](./assets/deployment.yaml) file to match the following:

    ```yaml
    apiVersion: apps/v1
    kind: Deployment
    metadata:
      name: contoso-website
    spec:
      selector: # define the wrapping strategy
        matchLabels: # match all pods with the defined labels
          app: contoso-website # labels follow the `name: value` template
      template:
        metadata:
          labels:
            app: contoso-website
        spec:
          nodeSelector:
            kubernetes.io/os: linux
          containers:
            - image: mcr.microsoft.com/mslearn/samples/contoso-website
              name: contoso-website
              resources:
                requests:
                  cpu: 100m
                  memory: 128Mi
                limits:
                  cpu: 250m
                  memory: 256Mi
              ports:
                - containerPort: 80
                  name: http
    ```

## Apply the Manifest
[Back to Top](#deploy-to-aks)

1. In the terminal, run the `kubectl apply` command to submit the deployment manifest to your cluster:

    ```bash
    kubectl apply -f ./deployment.yaml
    ```

    The command should output a result similar to the following example:

    ```
    deployment.apps/contoso-website created
    ```

2. Run the `kubectl get deploy` command to check if the deployment was successful:

    ```bash
    kubectl get deploy contoso-website
    ```

    The command should output a table similar to the following example:

    NAME | READY | UP-TO-DATE | AVAILABLE | AGE
    -----|-------|------------|-----------|----
    contoso-website | 0/1 | 1 | 0 | 16s

3. Run the `kubectl get pods` command to check if the pod is running.

    The command should output a table similar to the following example:

    NAME | READY | STATUS | RESTARTS | AGE
    -----|-------|--------|----------|----
    contoso-website-{uuid} | 1/1 | Running | 0 | 2m5s

## Create the Service Manifest
[Back to Top](#deploy-to-aks)

1. Login to the Azure CLI

    ```bash
    az login --use-device-code
    ```

2. Create a manifest file for the Kubernetes service called [`service.yaml`](./assets/service.yaml):

    ```bash
    touch service.yaml
    ```

3. Open [`service.yaml`](./assets/service.yaml) in VS Code and add the following:

    ```bash
    code ./service.yaml
    ```

    ```yaml
    apiVersion: v1
    kind: Service
    metadata:
      name: contoso-website
    ```

    In this code, you added the first two keys to tell Kubernetes the `apiVersion` and `kind` of manifest you're creating. The `name` is the name of the service. You'll use it to identify and query the service information when you use `kubectl`.

4. You define how the service will behave in the specification section of the manifest file. The first behavior you need to add is the type of service. Set the `type` key to `clusterIP`.

    Update the [`service.yaml`](./assets/service.yaml) file to match the following:

    ```yaml
    apiVersion: v1
    kind: Service
    metadata:
      name: contoso-website
    spec:
      type: ClusterIP
    ```

5. You define the pods the service will group and provide coverage by adding a `selector` section to the manifest file. Add the `selector`, and set the `app` key value to the `contoso-website` label of your pods as specified in your earlier deployment's manifest file.

    Update the [`service.yaml`](./assets/service.yaml) file to match the following:

    ```yaml
    apiVersion: v1
    kind: Servie
    metadata:
      name: contoso-website
    spec:
      type: ClusterIP
      selector:
        app: contoso-website
    ```

6. You define the port-forwarding rules by adding a `ports` section to the manifest file. The service must accept all TCP requests on port 80 and forward the request to the HTTP target port for all pods matching the selector value defined earlier.

    Update the `service.yaml` file to match the following:

    ```yaml
    apiVersion: v1
    kind: Service
    metadata:
      name: contoso-website
    spec:
      type: ClusterIP
      selector:
        app: contoso-website
      ports:
        - port: 80 # SERVICE exposed port
          name: http # SERVICE port name
          protocol: TCP # the protocol the SERVICE will listen to
          targetPort: http # port to forward to in the POD
    ```

## Deploy the Service
[Back to Top](#deploy-to-aks)

1. Run the `kubectl apply` command to submit thet service manifest to your cluster:

    ```bash
    kubectl apply -f ./service.yaml
    ```

    The command should output a result similar to the following example:

    ```
    service/contoso-website created
    ```

2. Run the `kubectl get service` command to check if the deployment was successful:

    ```bash
    kubectl get service contoso-website
    ```

    The command should output a result similar to the following example. Make sure the column `CLUSTER-IP` is filled with an IP address and the column `EXTERNAL-IP` is `<none>`. Also, make sure the column `PORT(S)` is defined to `80/TCP`:

    NAME | TYPE | CLUSTER-IP | EXTERNAL-IP | PORT(S) | AGE
    -----|------|------------|-------------|---------|----
    contoso-website | ClusterIP | 10.0.113.130 | \<none\> | 80/TCP | 16s

    With the external IP set to `<none>`, the application isn't available to external clients. The service is only available to the internal cluster.

## Create an Ingress Manifest
[Back to Top](#deploy-to-aks)

To expose the website to the world via DNS, you must create an ingress controller.

1. Create a manifest file for the Kubernetes service called [`ingress.yaml`](./assets/ingress.yaml):

    ```bash
    touch ingress.yaml
    ```

2. Open the [`ingress.yaml`](./assets/ingress.yaml) file in code and add the following section:

    ```bash
    code ./ingress.yaml
    ```

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: contoso-website
    ```

    In this code, you added the first two keys to tell Kubernetes the `apiVersion` and `kind` of manifest you're creating. The `name` is the name of the ingress. You'll use it to identify and query the ingress information when you use `kubectl`.

3. Create an `annotations` key inside the `metadata` section of the manifest file called to use the HTTP application routing add-on for this ingress. Set the key to `kubernetes.io/ingress.class` and a value of `addon-http-application-routing`.

    Update the [`ingress.yaml`](./assets/ingress.yaml) file to match the following:

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: contoso-website
      annotations:
        kubernetes.io/ingress.class: addon-http-application-routing
    ```

4. Set the FQDN of the host allowed access to the cluster.

    Run the `az network dns zone list` command to query the Azure DNS zone list:

    **Saved as [`query-dns-zone.sh`](./assets/query-dns-zone.sh)**

    ```bash
    az aks show \
      -g $RESOURCE_GROUP \
      -n $CLUSTER_NAME \
      -o tsv \
      --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
    ```

5. Copy the output and update the [`ingress.yaml`](./assets/ingress.yaml) file to match the following:

    > Replace the `<zone-name>` placeholder value with the `ZoneName` value copied from above.

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: contoso-website
      annotations:
        kubernetes.io/ingress.class: addon-http-application-routing
    spec:
      rules:
        - host: contoso.<zone-name> # which host is allowed to enter the cluster
    ```

6. Add the back-end configuration to your ingress rule. Create a key named `http` and allow the `http` protocol to pass through. Then, define the `paths` key that will allow you to filter whether this rule applies to all paths or the website or only some of them.

    Update the [`ingress.yaml`](./assets/ingress.yaml) file to match the following:

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: contoso-website
      annotations:
        kubernetes.io/ingress.class: addon-http-application-routing
    spec:
      rules:
        - host: contoso.<zone-name> # which host is allowed to enter the cluster
          http:
            paths:
              - backend: # how the ingress will handle the requests
                  service:
                    name: contoso-website #which service the request will be forwarded to
                    port:
                      name: http # which port in that service
                path: / # which path is this rule referring to
                pathType: Prefix # see more at https://kubernetes.io/docs/concepts/services-networking/ingress/#path-types
    ```

## Deploy the Ingress
[Back to Top](#deploy-to-aks)

1. Run the `kubectl apply` command to submit the ingress manifest to your cluster:

    ```bash
    kubectl apply -f ./ingress.yaml
    ```

    The command should output a result similar to the following:

    ```
    ingress.extensions/contoso-website created
    ```

2. Run the `kubectl get ingress` command to check if the deployment was successful:

    ```bash
    kubectl get ingress contoso-website
    ```

    The command should output a result similar to the following example:

    NAME | CLASS | HOSTS | ADDRESS | PORTS | AGE
    -----|-------|-------|---------|-------|----
    contoso-website | \<none\> | contoso.<zone-name>.aksapp.io | 4.236.210.21 | 80 | 36s

    Make sure the `ADRESS` column of the output is filled with an IP address. That's the address of your cluster.

    > There can be a delay between the creation of the ingress and the creation of the zone record. It can take up to five minutes for zone records to propogate.

3. Open your browser, and go to the FQDN described in the output. You should see a website that looks like the following example screenshot:

    ![contoso-website](https://learn.microsoft.com/en-us/training/modules/aks-deploy-container-app/media/7-website-success.png)

## Clean Up Resources
[Back to Top](#deploy-to-aks)

1. Open the [Azure Portal](https://portal.azure.com).

2. Select **Resource groups** on the left.

3. Find the **rg-contoso-video** resource group, or the resource group name you used, and select it.

4. On the **Overview** tab of the resource group, select **Delete resource group**.

5. Enter the name of the resource group to config. Select **Delete** to delete all of the resources you created in this module.

6. Finally, run the `kubectl config delete-context` command to remove the deleted clusters context. Here's an example of the complete command> Remember to replace the name of the cluster with your cluster's name:

    ```bash
    kubectl config delete-context aks-contoso-video
    ```

    If successful, the command returns the following:

    ```
    deleted context aks-contoso-video from /home/user/.kube/config
    ```