namespace PCFromScratch.DBModels;

public class RamGeneration
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public RamGeneration(string name) : this(Guid.CreateVersion7(), name)
    {
    }

    public RamGeneration(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private RamGeneration()
    {
    }
}