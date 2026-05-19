using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class Motherboard
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }

    public required Guid ManufacturerId { get; set; }
    public required Guid SocketId { get; set; }
    public required Guid ChipsetId { get; set; }
    public required Guid RamGenerationId { get; set; }

    public required Manufacturer Manufacturer { get; set; }
    public required CpuSocket Socket { get; set; }
    public required MotherboardChipset Chipset { get; set; }
    public required RamGeneration RamGeneration { get; set; }

    public MotherboardFormFactor FormFactor { get; set; }
    public int RamSlots { get; set; }
    public int RamFrequency { get; set; }
    
    public ICollection<MotherboardHeader> Headers { get; set; } = new HashSet<MotherboardHeader>();
    public ICollection<MotherboardExtension> MotherboardExtensions { get; set; } = [];
}