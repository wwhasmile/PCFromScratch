namespace PCFromScratch.DBModels;

public class CpuSocket
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public CpuSocket(string name) : this(Guid.NewGuid(), name)
    {
    }

    public CpuSocket(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private CpuSocket()
    {
    }
}
