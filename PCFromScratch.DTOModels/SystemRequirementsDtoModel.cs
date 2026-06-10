namespace PCFromScratch.DTOModels;

public record struct SystemRequirementsDtoModel(Guid? CpuBenchmark, Guid? GpuBenchmark, int RamInMegabytes,
        int SpaceOnDiskInGigabytes, bool SsdRequired);