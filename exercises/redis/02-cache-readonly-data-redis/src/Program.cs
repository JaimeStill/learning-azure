using Microsoft.Extensions.Configuration;
using SportsTracker;
using StackExchange.Redis;
using System.Text.Json;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

string? connection = config.GetValue<string>("CacheConnection");

if (connection is not null)
{
    using ConnectionMultiplexer cache = ConnectionMultiplexer.Connect(connection);

    IDatabase db = cache.GetDatabase();

    bool setValue = await db.StringSetAsync("test:key", "cache this");
    Console.WriteLine($"SET: {setValue}");

    string? getValue = await db.StringGetAsync("test:key");

    if (getValue is not null)
        Console.WriteLine($"GET: {getValue}");

    long newValue = await db.StringIncrementAsync("counter", 50);
    Console.WriteLine($"INCR new value = {newValue}");

    CacheData data = new("Test", 333, DateTime.Now);

    bool setData = await db.StringSetAsync("data:test", JsonSerializer.Serialize(data));
    Console.WriteLine($"SET: {setData}");

    string? getResult = await db.StringGetAsync("data:test");

    if (getResult is not null)
    {
        CacheData? getData = JsonSerializer.Deserialize<CacheData>(getResult);

        if (getData is not null)
            Console.WriteLine($"GET: {getData.ToString()}");
    }

    RedisResult result = await db.ExecuteAsync("ping");
    Console.WriteLine($"PING = {result.Type} : {result}");

    result = await db.ExecuteAsync("flushdb");
    Console.WriteLine($"FLUSHDB = {result.Type} : {result}");
}