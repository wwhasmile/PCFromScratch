using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class Motherboard
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required Manufacturer Manufacturer { get; set; }
    public required Socket Socket { get; set; }
    public required MotherboardChipset Chipset { get; set; }
    public MotherboardFormFactor FormFactor { get; set; }
    public required RamGeneration RamGeneration { get; set; }
    public int RamSlots { get; set; }
    public int RamFrequency { get; set; }
    public required ICollection<Pin> Pins { get; set; }
    public required ICollection<MotherboardExtension> MotherboardExtensions { get; set; }
}