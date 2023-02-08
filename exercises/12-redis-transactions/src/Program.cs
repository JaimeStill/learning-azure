using ServiceStack.Redis;

if (args.Length > 0)
{
    try
    {
        string connection = args[0];
        bool result = false;

        using RedisClient client = new(connection);
        using IRedisTransaction transaction = client.CreateTransaction();

        transaction.QueueCommand(c => c.Set("favorites:food", "pizza"));
        transaction.QueueCommand(c => c.Set("favorites:drink", "coffee"));

        transaction.QueueCommand(c => ((RedisNativeClient)c).Expire("favorites:food", 15));
        transaction.QueueCommand(c => ((RedisNativeClient)c).Expire("favorites:drink", 15));

        result = transaction.Commit();

        Console.WriteLine(
            result
                ? "Transaction committed"
                : "Transaction failed to commit"
        );

        string food = client.GetValue("favorites:food");
        string drink = client.GetValue("favorites:drink");

        Console.WriteLine($"Favorite Food: {food} - Favorite Drink: {drink}\n");
        Console.WriteLine("Cached preferences expire in 15 seconds\n");

        for (int i = 15; i > 0; i--)
        {
            Console.WriteLine($"{i} seconds remaining");
            await Task.Delay(1000);
        }

        food = client.GetValue("favorites:food");
        drink = client.GetValue("favorites:drink");
        
        Console.WriteLine("\nfavorites:food and favorites:drink expired");
        Console.WriteLine($"Values: {food} - {drink}");
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