namespace PCFromScratch.DBModels;

public class MotherboardPin
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public MotherboardPin(string name) : this(Guid.CreateVersion7(), name)
    {
    }

    public MotherboardPin(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private MotherboardPin()
    {
    }
}