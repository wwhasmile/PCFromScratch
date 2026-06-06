using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class MotherboardRenamedForOmnissiah
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Link { get; set; }

    public required string Socket { get; set; }
    public required MotherboardFormFactor FormFactor { get; set; }
    public required string Chipset { get; set; }

    public required string RamGeneration { get; set; }
    public int RamSlots { get; set; }
    public int RamFrequency { get; set; }
    public bool HasM2Slot { get; set; }
    public string? ImageUrl { get; set; }
    public int MinPrice { get; set; }
    public int MaxPrice { get; set; }
    public ICollection<Offer> Offers { get; set; } = new HashSet<Offer>();
}