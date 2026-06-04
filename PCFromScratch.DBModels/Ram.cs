namespace PCFromScratch.DBModels;

public class Ram
{
    public Guid Id { get; set; }
    public required string Model { get; set; }
    public string? Submodel { get; set; }
    public required string Link { get; set; }

    public int Amount { get; set; }
    public int Sticks { get; set; }

    public float Voltage { get; set; }
    public required string Generation { get; set; }
    public int Frequency { get; set; }
    public byte[]? Image { get; set; }
    public required string PriceRange { get; set; }
    public HashSet<Offer> Offers { get; set; } = new HashSet<Offer>();
}