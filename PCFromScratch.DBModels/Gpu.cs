namespace PCFromScratch.DBModels;

public class Gpu
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public int Tdp { get; set; }
}