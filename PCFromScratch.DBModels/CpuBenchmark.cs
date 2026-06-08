namespace PCFromScratch.DBModels;

public class CpuBenchmark
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Score { get; set; }
}