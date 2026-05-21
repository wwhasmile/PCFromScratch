using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class GpuArchitecture
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
}