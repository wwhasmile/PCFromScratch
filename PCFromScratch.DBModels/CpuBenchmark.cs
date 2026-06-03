namespace PCFromScratch.DBModels;

public class CpuBenchmark (Guid id, string name, int score)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public int Score { get; set; } = score;
}