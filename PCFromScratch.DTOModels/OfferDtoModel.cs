namespace PCFromScratch.DTOModels;

public record struct OfferDtoModel(string Shop, decimal Price, string? City)
{
    public string Display => $"{Shop} - {Price:C} ({City ?? "N/A"})";
}