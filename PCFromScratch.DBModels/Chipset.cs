namespace PCFromScratch.DBModels;

public class Chipset
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Chipset(string name) : this(Guid.CreateVersion7(), name)
    {
    }

    public Chipset(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private Chipset()
    {
    }
}
