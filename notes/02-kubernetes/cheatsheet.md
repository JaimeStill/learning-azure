# Kubernetes Cheatsheet

```bash
# create a deployment file
touch deploy.yaml
```

> [deploy.yaml](./assets/deploy.yaml)

```bash
# Deploy based on deploy.yaml
kubectl apply -f deploy.yaml

# test deployed service
# after starting, http://[external-ip-address]/WeatherForecast
kubectl get service mymicroservice --watch

# scale to two instances
kubectl scale --replicas=2 deployment/mymicroservice

# get all running containers
kubectl get pods

# delete container instance
kubectl delete pod {name}
```