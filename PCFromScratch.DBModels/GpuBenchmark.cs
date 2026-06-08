namespace PCFromScratch.DBModels;

public class GpuBenchmark
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Score { get; set; }
}