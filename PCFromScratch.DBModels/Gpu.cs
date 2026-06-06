namespace PCFromScratch.DBModels;

public class Gpu
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Link { get; set; }
    public int Tdp { get; set; }
    public int Length { get; set; }
    public string? ImageUrl { get; set; }
    public int MaxPrice { get; set; }
    public int MinPrice { get; set; }
    public ICollection<Offer> Offers { get; set; } = new HashSet<Offer>();
}