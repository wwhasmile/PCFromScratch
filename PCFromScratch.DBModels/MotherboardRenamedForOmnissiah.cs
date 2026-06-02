using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class MotherboardRenamedForOmnissiah
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public required string Socket { get; set; }
    public required MotherboardFormFactor FormFactor { get; set; }
    public required string Chipset { get; set; }

    public required string RamGeneration { get; set; }
    public int RamSlots { get; set; }
    public int RamFrequency { get; set; }

    public ICollection<MotherboardHeader> Headers = new HashSet<MotherboardHeader>();
    public ICollection<MotherboardExtension> Extensions = new HashSet<MotherboardExtension>();
}