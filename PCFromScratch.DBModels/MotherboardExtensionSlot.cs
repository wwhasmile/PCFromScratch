namespace PCFromScratch.DBModels;

public class MotherboardExtensionSlot
{
    public required Guid MotherboardId { get; set; }
    public required Guid ExtensionId { get; set; }

    public virtual required Motherboard Motherboard { get; set; }
    public virtual required MotherboardExtension Extension { get; set; }

    public int Count { get; set; }
}