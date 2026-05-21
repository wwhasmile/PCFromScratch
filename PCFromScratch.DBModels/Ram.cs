namespace PCFromScratch.DBModels;

public class Ram
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public int Amount { get; set; }
    public int Sticks { get; set; }

    public required string Generation { get; set; }
    public int Frequency { get; set; }
}