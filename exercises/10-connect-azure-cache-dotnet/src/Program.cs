using StackExchange.Redis;

if (args.Length > 0)
{
    try
    {
        string connection = args[0];

        using ConnectionMultiplexer cache = ConnectionMultiplexer.Connect(connection);
        IDatabase db = cache.GetDatabase();

        RedisResult result = await db.ExecuteAsync("ping");
        Console.WriteLine($"Ping = {result.Type} : {result}");

        bool setValue = await db.StringSetAsync("test:key", "100");
        Console.WriteLine($"SET: {setValue}");

        string? getValue = await db.StringGetAsync("test:key");

        if (getValue is not null)
            Console.WriteLine($"GET: {getValue}");
    }
    catch (Exception ex)
    {
        throw new Exception("An error occured interfacing with Azure Cache for Redis.", ex);
    }
}
else
{
    throw new Exception("No connection string was provided");
}