namespace PCFromScratch.DBModels;

public class InternalDrive
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public int Capacity { get; set; }
    public required string Type { get; set; }
    public required string FormFactor { get; set; }
    public required string Port { get; set; }
}