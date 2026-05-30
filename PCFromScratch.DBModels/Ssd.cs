namespace PCFromScratch.DBModels;

public class Ssd
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public int Size { get; set; }
    public required string FormFactor { get; set; }
    public required string Port { get; set; }
}