namespace PCFromScratch.DBModels;

public class PsuConnector
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
    public int Count { get; set; }
}