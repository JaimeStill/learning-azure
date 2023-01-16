# eShop Architecture

![eshop-architecture](https://learn.microsoft.com/en-us/training/aspnetcore/media/microservices/eshop-architecture.png)

## Identity

The *identity* service serves as a Security Token Service (STS). The identity service is a containerized ASP.NET Core project that uses [IdentityServer 4](https://identityserver4.readthedocs.io/en/latest/), a popular OpenID Connect and OAuth 2.0 framework for ASP.NET Core. An alternative is to use [Azure Active Directory](https://azure.microsoft.com/en-us/products/active-directory/).

## Event Bus

You use an event bus for asynchronous messaging and event-driven communication. The preceding architecture diagram depicts [RabbitMQ](https://www.rabbitmq.com/) in a Docker container deployed to AKS, but a service suchy as [Azure Service Bus](https://azure.microsoft.com/en-us/products/service-bus/) would also be appropriate.

![eventbus-implementation](https://learn.microsoft.com/en-us/training/aspnetcore/microservices-aspnet-core/media/3-solution-architecture/eventbus-implementation.png)

The preceding diagram depicts the publish/subscribe (commonly shortented to *pub-sub*) pattern used with the event bus. Any service can publish an event to the event bus. Each service is responsible for subscribing to the messages relevant to its domain. The services each call an `AddEventBus` extension method in the `ConfigureServices` method of *Startup.cs*. This method establishes a connection to the event bus, and registers the appropriate event handlers for that service's domain.

## API Gateway

API gateways enhance security and decouple back-end services from individual clients.

HTTP requests from the client app to the microservices are routed through the API gateway, which is an implementation of the [Backends-For-Frontends](https://learn.microsoft.com/en-us/azure/architecture/patterns/backends-for-frontends) pattern.

Implement basic routing configurations by using the NGINX reverse proxy. THe ASP.NET Core web API named *Web.Shopping.HttpAggregator* combines multiple requests into a single request. This is a pattern known as [*gateway aggregation*](https://learn.microsoft.com/en-us/azure/architecture/patterns/gateway-aggregation).

For real-world scenarios, use managed API gateway services like [Azure API Management](https://azure.microsoft.com/en-us/products/api-management/).

## Other Services

There are a few services in the Kubernetes deployment that aren't represented in the diagram. Services such as [Seq](https://datalust.co/seq) (for unified logging) and the *WebStatus* web app are also present in the deployment.

## Health Checks

The open-source project [AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) provides various custom health check implementation for ASP.NET Core projects. The [MongoDB implmentation](https://www.nuget.org/packages/AspNetCore.HealthChecks.MongoDb) is just one example.