using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class Motherboard
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required Manufacturer Manufacturer { get; set; }
    public required CpuSocket Socket { get; set; }
    public required MotherboardChipset Chipset { get; set; }
    public MotherboardFormFactor FormFactor { get; set; }
    public required RamGeneration RamGeneration { get; set; }
    public int RamSlots { get; set; }
    public int RamFrequency { get; set; }
    public required ICollection<MotherboardPin> Pins { get; set; }
    public required ICollection<MotherboardExtension> MotherboardExtensions { get; set; }
}