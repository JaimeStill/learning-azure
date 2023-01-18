# Explore the Functionality of a Kubernetes Cluster

Your goal in this exercise is to explore a Kubernetes installation with a single-node cluster. You're going to configure a *MicroK8s* environment that's easy to setup and teardwon. Then, you'll deploy an NGINX website and scale it out to multiple instances. Finally, you'll go through the steps to delete the running pods and clean up the cluster.

Keep in mind that there are other options, such a MiniKube and Kubernetes support in Docker, to do the same.

## MicroK8s

An option for deploying a single-node Kubernetes cluster as a single package to target workstations and Internet of Things (IoT) devices. Canonical, the creator of Ubuntu Linux, originally developed and maintains MicroK8s.

### MicroK8s WSL Install

* [Enable systemd in WSL](https://devblogs.microsoft.com/commandline/systemd-support-is-now-available-in-wsl/#set-the-systemd-flag-set-in-your-wsl-distro-settings)
* `sudo snap install microk8s --classic`
