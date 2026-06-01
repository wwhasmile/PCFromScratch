namespace PCFromScratch.DTOModels;

public class PCDtoModel
{
    public Guid Cpu { get; set; }
    public Guid Motherboard { get; set; }
    public Guid Gpu { get; set; }
    public Guid Ram { get; set; }
    public Guid Cooler { get; set; }
    public HashSet<Guid> InternalDrives { get; set; } = [];
    public Guid Psu { get; set; }
}
