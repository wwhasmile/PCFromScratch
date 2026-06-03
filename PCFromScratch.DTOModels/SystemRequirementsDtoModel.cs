namespace PCFromScratch.DTOModels;

public class SystemRequirementsDtoModel
{
    public Guid? CpuBenchmark { get; set; }
    public Guid? GpuBenchmark { get; set; }
    public int RamInMegabytes { get; set; }
    public int SpaceOnDiskInGigabytes { get; set; }
    public bool SsdRequired { get; set; }
}