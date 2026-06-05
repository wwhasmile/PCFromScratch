namespace PCFromScratch.DBModels;

public class Offer
{
    public required Guid Id { get; set; }
    public required string ShopName { get; set; }
    public decimal Price { get; set; }
    public string? City { get; set; }
}