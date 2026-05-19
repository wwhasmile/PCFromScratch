namespace PCFromScratch.DBModels;

public class Manufacturer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Manufacturer(string name) : this(Guid.CreateVersion7(), name)
    {
    }

    public Manufacturer(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private Manufacturer()
    {
    }
}
