# Redis

Redis (REmote DIctionary Server) is a widely used, open-source caching solution.It's a key-value datastore that runs in memory, so it's very responsive.

Caching is the act of storing frequently accessed data in memory very close to the application that consumes the data. Caching is used to increase performance and reduce the load on your servers.

Typically, organizations use Redis to complement their database apps. Combining Redis with back-end databases enables you to significantly improve your apps' performance.

Any caching solution should address four key requirements:

* **Performance.** The primary requirement for any caching solution is to improve performance, even under high loads. Ideally, it should increase throughput and reduce latency.  

* **Scalability.** A system must respond to load changes promptly. Scaling should be automatic and occur without downtime.  

* **Availability.** Any caching solution must be highly available. This helps ensure that your apps can deliver at peak performance, even if component failures occur.  

* **Support for geographic distribution.** It's essential that a caching solution provides the same performance and scaling benefits everywhere in the world. This can be challenging if your data is geographically dispersed.  

## Data Types

Redis supports various data types, all oriented around *binary safe* strings. You can use any binary sequence for a value, from a string like "i-love-rocky-road" to the contents of an image file. An empty string is also a valid value.

* Binary-safe strings (most common)
* Lists of strings
* Unordered sets of strings
* Hashes
* Sorted sets of strings
* Maps of strings

Each data value is associated with a *key* that can be used to look up the value from the cache. Redis works best with smaller values (100k or less), so consider chopping bigger data into multiple keys. Storing larger values is possible (up to 500MB), but increases network latency and can cause caching and out-of-memory issues if the cache isn't configured to expire old values.

## Keys

Redis keys are also binary-safe strings. Here are some guidelines for choosing keys:

* Avoid long keys. They take up more memory and require longer lookup times because they have to be compared byte-by-byte. If you want to use a binary blob as the key, generate a unique hash and use that as the key instead. The maximum size of a key is 512M, but you should *never* use a key that size.

* Use keys that can identify the data. For example, `sport:football;date:2008-02-02` would be a better key than `fb:8-2-2`. The former is more readable and the extra size is negligible. Find the balance between size and readability.

* Use a convention. A good one is `object:id`, as in `sport:football`.

## Data Storage

Data in Redis is stored in **nodes** and **clusters**.

**Nodes** are a space in Redis where your data is stored.

**Clusters** are sets of three or more nodes across which your dataset is split. Clusters are useful because your operations will continue if a node fails or is unable to communicate with the rest of the cluster.

## Azure Cache for Redis

Azure Cache for Redis enables you to implement Redis as a fully managed service.

> Azure Cache for Redis offers both Redis open-source (OSS Redis) and a commercial product from Redis Labs (Redis Enterprise) as a managed service, depending on the tier you select.

Azure Cache for Redis can help improve performance in apps that interface with many database solutions, including Azure SQL Database, Azure Cosmos DB, and Azure Database for MySQL:

![distributed-cache](https://learn.microsoft.com/en-us/training/modules/intro-to-azure-cache-for-redis/media/distributed-cache.png)

Azure Cache for Redis can help you improve your apps' performance and scalability by copying frequently accessed data and storing it in memory.

Azure Cache for Redis can be used as an event aggregation pipeline for microservices in a more comprehensive overall solution.

Azure Cache for Redis can also be used as a message broker between various application components.

Azure Cache for Redis provides the following application architecture patterns:

* **Data cache.** Databases often are too large to directly load into cache. That's why it's common to use the *cache-aside* pattern. It loads data into the cache only as needed.  

* **Content cache.** Most webpages contain static items, such as headers, footers, and banners. These items don't change very often. By using an in-memory content cache, you can provide quick access to static content as compared to accessing back-end datastores.  

* **Session store.** This pattern is often used with shopping carts or other data based on user history data. The reason is that web applications often associate these with user cookies. Storing too much data in a cookie can adversely affect performance. The reason is that apps often use cookies to query a back-end database for user data. Using an in-memory cache to store user-session information is faster than working with the backend database.  

* **Job and message queuing.** Apps frequently add tasks to a queue. This occurs when the tasks might take a long time to run. If a task contains long-running operations, they're typically queued to run in sequence. Azure Cache for Redis provides publish/subscribe, message streaming, or queue architectures to support this application pattern.  

    > Longer running operations are queued to process in sequence, often by another server.

* **Distributed transactions.** Sometimes apps require a series of commands to run on a back-end datastore as a single operation. Azure Cache for Redis supports running a batch of commands as a single transaction.  

    > All commands must either succeed or be rolled back to the initial state.

### Tiers

Azure Cache for Redis Tiers:

* The **Basic** tier runs on a single virtual machin (VM) and doesn't include a service-level agreement (SLA). This tier is based on an OSS Redis cache.

* The **Standard** tier runs on two replicated VMs and is based on an OSS Redis cache.

    > Standard and Basic are single-node caches. You should consider these tiers only for noncritical workloads.

* The **Premium** tier is deployed on more powerful VMs. This tier offers features such as higher throughput, lower latency, and better availability. This tier is based on an OSS Redis cache.

* The **Enterprise** tier offers higher availability than the Premium tier and a high-performance cache that's powered by Redis Labs' Redis Enterprise software.

* The **Enterprise Flash** tier offers a cost-effective alternative to the Enterprise tier and is also powered by Redis Labs' Redis Enterprise software. This tier extends Redis data storage to nonvolatile memory, which reduces overall memory cost per gigabyte (GB).

All tiers support the following features:

* Data encryption in transit
* Network isolation
* Scaling

The Premium, Enterprise, and Enterprise Flash tiers also support other advanced features, including:

* **Clustering.** Provides for high-availability and load distribution.
* **Data persistence.** Allows you to persist data in Redis. This enables you to take snapshots and back up data. You can then load these snapshots should a hardware failure occur.
* **Zone redundancy.** Provides higher resilience and availability because the VMs are spread across multiple availability zones.
* **Geo-replication.** Links two Azure Cache for Redis instances and creates a data-replication relationship. This replication provides a potential disaster-recovery solution.
* **Import/Export.** Enables you to import data into, or export data from, Azure Cache for Redis. You can import or export an Azure Cache for Redis Database (RDB) snapshot from a premium cache to an Azure Storage Account blob.

The following features are only available in the Enterprise tiers:

* **RediSearch.** Provides a powerful indexing and querying engine with a full-text search engine.
* **RedisBloom.** Provides support for probabilistic data structures.
* **RedisTimeSeries.** Enables you to ingest and query large quantities of data with very high performance.
* **Active Geo-Replication.** Implements conflict-free replicated data types, and supports writes to multiple cache instances. Manages merging of changes and conflict resolution when necessary.

## Use Cases

Azure Cache for Redis provides a number of use-cases that help improve the performance and scalability of apps that rely heavily on back-end data stores. You'll learn what the following Azure Cache for Redis use-cases provide:

* Distributed cache
* Session store
* Message broker
* Cloud migration

### Distributed Cache

The distributed cache use-case in Azure Cache for Redis helps improve your apps' response times by copying frequently-accessed data to a cache. This cache has lower latency and provides for higher throughput than the primary datastore. The distributed cache feature:

* Accelerates application responsiveness
* Helps reduce load on primary datastores and compute resources
* Integrates with many Azure databases, including Azure SQL and Azure Cosmos DB

You can use distributed cache to:

* Manage spikes in traffic
* Cache and provide commonly accessed data to users
* Help reduce compute load on your databases
* Locate content geographically closer to users
* Provide for output caching

### Session Store

Your session-oriented apps require the ability to store and access temporary session data when users sign in and remain active on your apps. The session store use-case in Azure Cache for Redis:

* Manages up to hunders of thousands of simultaneous users
* Makes data-replication options available to help provide for maximum reliability
* Helps reduce costs, as it's typically more cost-effective and scalable than alternative database or storage options

You can use session store to:

* Help facilitate eCommerce shopping carts
* Store user cookies
* Maintain user login and session state data
* Enable IoT telemetry

### Message Broker

Apps built on microservices often need to asynchronously communicate. Azure Cache for Redis can implement a publish/subscribe or queue architecture that can help enable fast and reliable communication between these microservices. The Azure Cache for Redis message broker:

* Provides a temporary data store with minimal overhead and cost
* Supports TLS encryption for data in transit
* Provides network isolation for secure communication between your services

You can use the message broker use-case to:

* Publish news, financial data, or application updates to users
* Manage chat messages
* Enable communication between microservices

![message-broker](https://learn.microsoft.com/en-us/training/modules/intro-to-azure-cache-for-redis/media/message-broker.png)

### Cloud Migration

If you're moving from an on-premises cache to a managed service, a critical factor is how to move content to the managed service. Azure Cache for Redis helps migrate data to the cloud and also:

* Enables both import and export of RDB files
* Provides compatibility with open-source Redis to help simplify migration
* Provides a fully managed service that manages:
    * Patching
    * Updates
    * Provisioning
    * Scaling
    * Setup

You can use cloud-migration in Azure Cache for Redis to:

* Migrate your apps from your on-premises environment to the cloud
* Help modernize your current infrastructure as a service (IaaS) apps through the benefits of platform as a service (PaaS) services.

A typical process proceeds as follows:

1. From an existing on-premises Redis cache, you export the cache to an RDB file
2. You create an Azure Cache for Redis instance
3. You import the RDB into this instance
4. You configure your new application to point to your Azure Cache for Redis instance

![cloud-migrate](https://learn.microsoft.com/en-us/training/modules/intro-to-azure-cache-for-redis/media/cloud-migrate.png)

## Accessing the Redis Instance

Redis has a command-line tool for interacting with an Azure Cache for Redis as a client. The tool is available for Windows platforms by downloading the [Redis command-line tool for Windows](https://github.com/MSOpenTech/redis/releases/). If you want to run the command-line tool on another platform, download Azure Cache for Redis from https://redis.io/download.

Redis supports a set of known commands. A command is typically issued as `COMMAND parameter 1 parameter2 parameter3`.

Common commands:

Command | Description
--------|------------
`ping` | Ping the server. Returns "PONG".
`set [key] | [value]` | Sets a key/value in the cache. Returns "OK" on success.
`get [key]` | Gets a value from the cache.
`exists [key]` | Returns `1` if the **key** exists in the cache, `0` if it doesn't.
`type [key]` | Returns the type associated to the value for the given **key**.
`incr [key]` | Increment the given value associated with **key** by `1`. The value must be an integer or double value. This returns the new value.
`incrby [key] [amount]` | Increment the given value associated with the **key** by the specified amount. The value must be an integer or double value. This returns the new value.
`del [key]` | Deletes the value associated with the **key**.
`flushdb` | Delete *all* keys and values in the database.

### Adding an Expiration Time to Values

Caching is important because it allows us to store commonly used values in memory. However, we also need a way to expire values when they are stale. In Redis this is done by applying a time to live (TTL) to a key.

When the TTL elapses, the key is automatically deleted, exactly as if the `del` command were issued. Here are some notes on TTL expirations:

* Expirations can be set using seconds or milliseconds precision.
* The expire time resolution is always 1 millisecond.
* Information about expires are replicated and persisted on disk, the time virtually passes when your Redis server remains stopped (this means that Redis saves the date at which a key will expire).

Example:

```
> set counter 100
OK
> expire counter 5
(integer) 1
> get counter
100
... wait ...
> get counter
(nil)
```

## Interact with Azure Cache for Redis by Using .NET

Typically, a client application will use a client library to form requests and execute commands on a Redis cache. You can get a list of client libraries directly from the Redis clients page.

A popular high-performance Redis client for the .NET language is [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis).

### Connecting

The host address, port number, and an access key are used to connect to a Redis server. Azure also offers a connection string for some Redis clients which bundles this data together into a single string. It will look something like the following (with the `cache-name` and `password-here` fields filled in with real values):

```
[cache-name].redis.cache.windows.net:6380,password=[password-here],ssl=True,abortConnect=False
```

You can pass this string to **StackExchange.Redis** to create a connection to the server.

THere are two additional parameters at the end:

* **ssl** - ensures that communication is encrypted.
* **abortConnection** - allows a connection to be created even if the server is unavailable at the moment.

There are several other [optional parameters]() you can append to the string to configure the client library.

### Creating a Connection

The main connection object in **StackExchange.Redis** is the `StackExchange.Redis.ConnectionMultiplexer` class. This object abstracts the process of connecting to a Redis server (or group of servers). It's optimized to manage connections efficiently and intended to be kept around while you need access to the cache.

You create a `ConnectionMultiplexer` instance using the static `ConnectionMultiplexer.Connect` or `ConnectionMultiplexer.ConnectAsync` method, passing in either a connection string or a `ConfigurationOptions` object.

Example:

```cs
using StackExchange.Redis;

string connectionString = "[cache-name].redis.cache.windows.net:6380,password=[password-here],ssl=True,abortConnection=False";
ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(connectionString);
```

Once you have a `ConnectionMultiplexer`, there are 3 primary things you might want to do:

* Access a Redis Database.
* Make use of the publisher / subscriber features of Redis.
* Access an individual server for maintenance or monitoring purposes.

### Accessing a Redis Database

The Redis database is represented by the `IDatabase` type. You can retrieve one using the `GetDatabase()` method:

```cs
IDatabase db = redisConnection.GetDatabase();
```

> The object returned from `GetDatabase` is a lightweight object and does not need to be stored. Only the `ConnectionMultiplexer` needs to be kept alive.

Once you have an `IDatabase` object, you can execute methods to interact with the cache. All methods have synchronous and asynchronous versions which return `Task` objects to make them compatible with the `async` and `await` keywords.

Example of storing a key/value in the cache:

```cs
bool wasSet = db.StringSet("favorite:flavor", "i-love-rocky-road");
```

The `StringSet` method returns a `bool` indicating whether the value was set (`true`) or not (`false`). We can then retrieve the value with the `StringGet` method:

```cs
string value = db.StringGet("favorite:flavor");
Console.WriteLine(value); // "i-love-rocky-road"
```

### Getting and Setting Binary Values

Recall that Redis keys and values are binary safe. These same methods can be used to store binary data. There are implicit conversion operators to work with `byte[]` types so you can work with the data naturally:

```cs
byte[] key = ...;
byte[] value = ...;

db.StringSet(key, value);

byte[] key = ...;
byte[] value = db.StringGet(key);
```

**StackExchange.Redis** represents keys using the `RedisKey` type. This class has implicit conversions to and from both `string` and `byte[]`, allowing both text and binary keys to be used without any complication. Values are represented by the `RedisValue` type. As with `RedisKey`, there are implicit conversions in place to allow you to pass `string` or `byte[]`.

### Other Common Operations

The `IDatabase` interface includes several other methods to work with the Redis cache. There are methods to work with hashes, lists, sets, and ordered sets. You can [read the source code](https://github.com/StackExchange/StackExchange.Redis/blob/master/src/StackExchange.Redis/Interfaces/IDatabase.cs) for the interface to see the full list.

Common methods that work with single keys:

Method | Description
-------|------------
`CreateBatch` | Creates a group of operations that will be sent to the server as a single unit, but not necessarily processed as a unit.
`CreateTransaction` | Creates a group of operations that will be sent to the server as a single unit and processed on the server as a single unit.
`KeyDelete` | Delete the key/value.
`KeyExists` | Return whether the given key exists in cache.
`KeyExpire` | Sets a time-to-live (TTL) expiration on a key.
`KeyRename` | Renames a key.
`KeyTimeToLive` | Returns the TTL for a key.
`KeyType` | Returns the string representation of the type of the value stored at key. The different types that can be returned are: string, list, set, zset and hash.

### Executing Other Commands

The `IDatabase` object has an `Execute` and `ExecuteAsync` method which can be used to pass textual commands to the Redis server. For example:

```cs
RedisResult result = db.Execute("ping");
Console.WriteLine(result.ToString()); // "PONG"
```

The `Execute` and `ExecuteAsync` methods return a `RedisResult` object which is a data holder that includes two properties:

* `Type` which returns a `string` indicating the type of the result - "STRING", "INTEGER", etc.
* `IsNull` a true/false value to detect when the result is `null`.

You can then use `ToString()` on the `RedisResult` to get the actual return value.

You can use `Execute` to perform any supported commands - for example, we can get all the clients connected to the cache ("CLIENT LIST"):

```cs
RedisResult result = await db.ExecuteAsync("client", "list");
Console.WriteLine($"Type = {result.Type}\r\nResult = {result}");
```

This would output all the connected clients:

```
Type = BulkString
Result = id=9469 addr=16.183.122.154:54961 fd=18 name=DESKTOP-AAAAAA age=0 idle=0 flags=N db=0 sub=1 psub=0 multi=-1 qbuf=0 qbuf-free=0 obl=0 oll=0 omem=0 ow=0 owmem=0 events=r cmd=subscribe numops=5
id=9470 addr=16.183.122.155:54967 fd=13 name=DESKTOP-BBBBBB age=0 idle=0 flags=N db=0 sub=0 psub=0 multi=-1 qbuf=0 qbuf-free=32768 obl=0 oll=0 omem=0 ow=0 owmem=0 events=r cmd=client numops=17
```

### Storing More Complex Values

Redis is oriented around binary safe strings, but you can cache off object graphs by serializing them to a textual format - typically XML or JSON. For example, perhaps for our statistics, we have a `GameStats` object which looks like:

```cs
public class GameStat
{
    public string Id { get; set; }
    public string Sport { get; set; }
    public DateTimeOffset DatePlayed { get; set; }
    public string Game { get; set; }
    public IReadOnlyList<string> Teams { get; set; }
    public IReadOnlyList<(string team, int score)> Results { get; set; }

    public GameStat(string sport, DateTimeOffset datePlayed, string game, string[] teams, IEnumerable<(string team, int score)> results)
    {
        Id = Guid.NewGuid().ToString();
        Sport = sport;
        DatePlayed = datePlayed;
        Game = game;
        Teams = teams.ToList();
        Results = results.ToList();
    }

    public override string ToString()
    {
        return $"{Sport} {Game} played on {DatePlayed.Date.ToShortDateString()} - " +
               $"{String.Join(',', Teams)}\r\n\t" + 
               $"{String.Join('\t', Results.Select(r => $"{r.team } - {r.score}\r\n"))}";
    }
}
```

We could use **System.Text.Json** to turn an instance of this object into a string:

```cs
GameStat stat = new("Soccer", new DateTime(2019, 7, 16), "Local Game",
                     new[] { "Team 1", "Team 2" },
                     new[] { ("Team 1", 2), ("Team 2", 1) });

string serializedStat = JsonSerializer.Serialize(stat);
bool added = db.StringSet("event:1950-world-cup", serializedStat);
```

We could retrieve it and turn it back into an object using the reverse process:

```cs
RedisResult result = db.StringGet("event:1950-world-cup");
GameState stat = JsonSerializer.Deserialize<GameStat>(result.ToString());
Console.WriteLine(stat.Sport); // "Soccer"
```

### Cleaning Up the Connection

Once you're done with the Redis connection, you can `Dispose` the `ConnectionMultiplexer`. This will close all connections and shutdown the communication with the server:

```cs
redisConnection.Dispose();
redisConnection = null;
```

## Transactions

There are times when you must guarantee that multiple operations execute together. Transactions in Redis work by queueing multiple commands to be executed as a group. When a transaction is executed, the commands queued inside of it are guaranteed to execute without any other commands from other clients interleaved between them.

To begin a transaction block, enter the `MULTI` command. Further commands will be queued and not executed immediately. Running the `EXEC` command will execute all of the queued commands as a transactional unit. If you decide you want to abort an open transaction while queueing commands, running the `DISCARD` command will close the transaction block without running *any* of the queued commands.

Redis transactions do not support the concept of rollback. If you queue a command with incorrect syntax into a transaction block, the block will remain open, but will automatically be discareded if you attempt to execute it with `EXEC`. Commands in a transaction that fail *during* execution (after `EXEC` is called) do not cause a transaction to be aborted or rolled back - Redis will still run all of the commands and consider the transaction to have completed successfully.

### Transactions with **ServiceStack.Redis**

**ServiceStack.Redis** is a C# library for interacting with Redis.

Transactions in **ServiceStack.Redis** are created by calling `IRedisClient.CreateTransaction()`. The `IRedisTransaction` object that is returned can have multiple commands queued into it with `QueueCommand()`. Calling `Commit()` on the transaction object will execute it.

`IRedisTransaction` objects are disposable, and will automatically issue a `DISCARD` command if disposed before calling `Commit()`. This feature works well with C#'s `using` blocks: if you don't commit a transaction for any reason, you can trust that the transaction will automatically be discarded so that the Redis connection can continue to be used.

Here's an example of using **ServiceStack.Redis** to create a transaction that can send a message that includes a picture URL and the contents of a text message:

```cs
public bool SendPictureAndText(string groupChatId, string text, string pictureUrl)
{
    using RedisClient client = new RedisClient(connection));
    using IRedisTransaction transaction = client.CreateTransaction();

    transaction.QueueCommand(c => c.PublishMessage(groupChatId, pictureUrl));
    transaction.QueueCommand(c => c.PublishMessage(groupChatId, text));
    
    return transaction.Commit();
}
```

## Data Expiration

Data is not always permanent, and there are times you would like to delete it at a specific time. Data expiration is a feature that can automatically delete a key and value in the cache after a set amount of time.

Data expiration is commonly used in situations where the data you're storing is short-lived. This is important beacuse Azure Cache for Redis is an in-memory database and you don't have as much memory available to use as you would if you were storing on disk. Because you have limited storage with Azure Cache for Redis, you want to make sure that you're only storing data that is important. If the data doesn't need to be around for a long time, make sure you set an expiration.

### How to Use Data Expiration in Azure Cache for Redis

There are different commands to implement and manage data expiration in Azure Cache for Redis:

* `EXPIRE`: Sets the timeout of a key in seconds
* `PEXPIRE`: Sets the timeout of a key in milliseconds
* `EXPIREAT`: Sets the timeout of a key using an absolute Unix timestamp in seconds
* `PEXPIREAT`: Sets the timeout of a key using an absolute Unix timestamp in milliseconds
* `TTL`: Returns the remaining time a key has to live in seconds
* `PTTL`: Returns the remaining time a key has to live in milliseconds
* `PERSIST`: Makes a key never expire

The most common command is `EXPIRE`.

### Example of Data Expiration using **ServiceStack.Redis** in C#

```cs
public static void SetGroupChatName(string groupChatId, string chatName)
{
    using RedisClient client = new(connection);

    // Create a key for group chat display names
    string key = $"{groupChatId}displayName";

    // Set the group chat display name
    redisClient.SetValue(key, chatName);

    // Set the expiration for one hour
    redisClient.Expire(key, 3600);
}
```

## Eviction Policies

Memory is the most critical resource for Azure Cache for Redis, because it's an in-memory database. You can run into problems when you begin adding data that exceeds the amount of memory available. Azure Cache for Redis supports eviction policies, which indicate how data should be handled when you run out of memory.

An eviction policy is a plan that determines how your data should be managed when you exceed the maximum amount of memory available. For example, using an eviction policy, you could tell Azure Cache for Redis to delete a random key to make room for the next data being inserted.

### Types of Eviction Policies

There are eight different eviction policies provided by Azure Cache for Redis. All of these perform an action when you attempt to insert data when you're out of memory:

* **noeviction.** *No eviction* policy. Returns an error message if you attempt to insert data.
* **allkeys-lru.** Removes the *least recently used* keys.
* **allkeys-random.** Removes *random* keys.
* **allkeys-lfu.** Evicts the *least frequently used* keys.
* **volatile-lru.** Removes the *least recently used* keys out of all the keys with an expiration set.
* **volatile-ttl.** Removes the keys with the shortest *time to live* based on the expiration set for them.
* **volatile-random.** Removes *random* keys that have an expiration set.
* **volatile-lfu.** Evicts the *least frequently used* keys out of all keys with an *expire* field set.

## Cache-aside Pattern

When building an application, you want to provide a great user experience, and part of that is quick data retrieval. Retrieving data from a database is typically a slow process, and if this data is read often, this could provide a poor user experience. The cache-aside pattern describes how you can implement a cache in conjunction with a database, to return the most commonly accessed data as quickly as possible.

The cache-aside pattern dictates that when you need to retrieve data from a data source, like a relational database, you should first check for the data in your cache. If the data is in you cache, use it. If the data is not in your cache, then query the database, and when you're returning the data back to the user, add it to your cace. This will then allow you to access the data from your cache the next time it's needed.

### When to Implement the Cache-aside Pattern

Reading data from a database is usually a slow process. It involves compliation of a complex query, preparation of a query execution plan, execution of the query, and then preparation of the result. In some cases, this process may read data from the disk as well. There are optimizations that can be done at the database level, like pre-compiling the queries, and loading some of the data in memory. However, execution of the query and preparation of the result will always happen when retrieving data from a database.

We can solve this problem by using the cache-aside pattern. In the cache-aside pattern, we still have the application and the database, but now we also have a cache. A cache stores its data in memory, so it doesn't have to interact with the file system. Caches also store data in very simple data structures, like key value pairs, so they don't have to execute complex queries to gather or maintain indexes when writing data. Because of this, a cache is typically more performant than a database. When you use an application, it will try to read data from the cache first. If the requested data is not in the cache, the application will retrieve it from the database, like it always has done. However, then it stores the data in the cache for subsequent requests. Next time any user requests the data, it will return it from the cache directly.

![cache-aside-set-cache](https://learn.microsoft.com/en-us/training/modules/work-with-mutable-and-partial-data-in-a-redis-cache/media/8-cache-aside-set-cache.png)

### How to Manage Updating Data

When you implement the cache-aside pattern, you introduce a small problem. Because your data is now stored in a cache and a data store, you can run into problems when you try to make an update. For example, to update your data, you would need to update both the cache and the data store.

The solution to this problem in the cache-aside pattern is to invalidate the data in the cache. When you update in your application, you should first delete the data in the cache, and then make the changes to the data source directly. By doing this, next time the data is requested, it won't be present in the cache, and the process will repeat.

![cache-aside-invalidate](https://learn.microsoft.com/en-us/training/modules/work-with-mutable-and-partial-data-in-a-redis-cache/media/8-cache-aside-invalidate.png)

### Considerations for Using the Cache-aside Pattern

Carefully consider which data to put in the cache. Not all data is suitable to be cached:

* **Lifetime**: For cache-aside to be effective, make sure that the expiration policy matches the access frequency of the data. Making the expiration period too short can cause applications to continually retrieve data from the data store and add it to the cache.

* **Evicting**: Caches have a limited size compared to typical data stores, and they'll evict data if necessary. Make sure you choose an appropriate eviction policy for your data.

* **Priming**: To make the cache-aside pattern effective, many solutions will prepopulate the cache with data that they think will be accessed often.

* **Consistency**: Implementing the cache-aside pattern doesn't guarantee consistency between the data store and the cache. Data in a data store can be changed without notifying the cache. This can lead to serious synchronization issues.

The cache-aside pattern is useful when you're required to access data frequently from a data source that uses a disk. Using the cache-aside pattern, you'll store important data in a cache to help increase the speed of retrieving it.

## Pub/Sub

Distributed microservices often respond to events that occur in other microservices. This pattern is commonly referred to as *event-driven architecture*.

An *event aggregator* is a middleware solution that aggregates the events across the entire solution in a scalable and straightforward to manage way. Here, you'll learn how the Pub/Sub feature of Azure Cache for Redis can serve as a middleware to simplify the communication between components of your application. Pub/Sub can help your application components subscribe to other's events and publish their own events.

The **Pub/Sub** feature of Azure Cache for Redis routes messages between application components. Microservcies can use this feature to subscribe to messages or publish messages. Azure Cache for Redis will handle the routing of messages to the appropriate destinations without each microservice knowing where each of their messages should go.

This feature can simplify the requirement of microservices reacting to events across the entire solution.

## Subscribing to Channels

Clients can subscribe toa  topic or range of topics used as a string value. In Redis nomenclature, topics are referred to as **channels**. For example, a client that wants to subscribe to the **staff.newhire** channel can use the appropriate command with the `staff.newhire` string value.

Once the client is subscribed, all messages in the **staff.newhire** channel will be sent to that specific client.

![pub-sub-channels](https://learn.microsoft.com/en-us/training/modules/azure-redis-publish-subscribe-streams/media/2-subscribe.svg)

Redis includes a `SUBSCRIBE` command used to subscribe to one or more channels. This command is flexible enough to subscribe to a space-delimited list of channels.

### Subscribe to a Single Known Channel

The most common use of the `SUBSCRIBE` command is to subscribe to a single channel:

```redis
SUBSCRIBE staff.newhire
```

### Subscribe to Multiple Known Channels

The `SUBSCRIBE` command can also be used to subscribe to multiple channels simultaneously. To subscribe to multiple channels, separate each channel with a single space in the channels list:

```redis
SUBSCRIBE orders.new orders.delete
```

### Subscribe to a Pattern of Channels

The `PSUBSCRIBE` command uses glob-style patterns to subscribe a client to any channel that matches the specific pattern. There are three primary operators that you can use in a glob-style pattern:

Operator | Description | Example | Matches | Does Not Match
---------|-------------|---------|---------|---------------
`?` | matches any single character | `l?arn` | `learn, loarn` | `larn, lern`
`*` | Matches any content (including none) | `lear*` | `learn, learaeiou` | `larn, lern`
`[]` | Matches only characters within the list | `le[ao]rn` | `learn, leorn` | `lern, leurn`

**Example:** use the `PSUBSCRIBE` command to subscribe to all channels that begin with a prefix of **inventory**:

```redis
PSUBSCRIBE inventory.*
```

**Example:** use the `PSUBSCRIBE` command to subscribe to all channels with the suffix of **.new**:

```redis
PSUBSCRIBE *.new
```

**Example:** use the `PSUBSCRIBE` command to subscribe to all channels that include **orders** or **staff** as a whole word in the name:

```redis
PSUBSCRIBE *orders* *staff*
```

## Unsubscribe From Channels

