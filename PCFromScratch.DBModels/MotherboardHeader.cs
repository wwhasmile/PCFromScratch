namespace PCFromScratch.DBModels;

public class MotherboardHeader
{
    public required Guid MotherboardId { get; set; }
    public required Guid PinId { get; set; }

    public required Motherboard Motherboard { get; set; }
    public required MotherboardPin Pin { get; set; }

    public int Count { get; set; }
}