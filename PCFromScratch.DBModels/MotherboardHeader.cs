namespace PCFromScratch.DBModels;

public class MotherboardHeader
{
    public required Guid MotherboardId { get; set; }
    public required Guid PinId { get; set; }

    public required Motherboard? Motherboard;
    public required MotherboardPin? Pin;

    public int Count { get; set; }
}