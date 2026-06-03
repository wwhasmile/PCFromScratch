namespace PCFromScratch.DBModels;

public class GpuBenchmark (Guid id, string name, int score)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public int Score { get; set; } = score;
}