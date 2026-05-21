namespace PCFromScratch.DBModels;

public class MotherboardPinSlot
{
    public required Guid MotherboardId { get; set; }
    public required Guid PinId { get; set; }

    public virtual required Motherboard Motherboard { get; set; }
    public virtual required MotherboardPin Pin { get; set; }

    public int Count { get; set; }
}