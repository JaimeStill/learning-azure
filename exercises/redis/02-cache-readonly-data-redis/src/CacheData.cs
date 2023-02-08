namespace SportsTracker;

public record CacheData
{
    public string Name { get; set; }
    public int Value { get; set; }
    public DateTime Date { get; set; }

    public CacheData(string name, int value, DateTime date)
    {
        Name = name;
        Value = value;
        Date = date;
    }

    public override string ToString() =>
        $"{Name}:{Value}:{Date.ToString("dd-MMM-yyyy")}";
}