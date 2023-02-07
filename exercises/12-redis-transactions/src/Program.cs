using ServiceStack.Redis;

if (args.Length > 0)
{
    try
    {
        string connection = args[0];
        bool result = false;

        using RedisClient client = new RedisClient(connection);
        using IRedisTransaction transaction = client.CreateTransaction();

        transaction.QueueCommand(c => c.Set("favorites:food", "pizza"));
        transaction.QueueCommand(c => c.Set("favorites:drink", "coffee"));

        result = transaction.Commit();

        Console.WriteLine(
            result
                ? "Transaction committed"
                : "Transaction failed to commit"
        );
    }
    catch (Exception ex)
    {
        throw new Exception("An error occurred interfacing with Azure Cache for Redis.", ex);
    }
}
else
{
    throw new Exception("No connection string was provided");
}