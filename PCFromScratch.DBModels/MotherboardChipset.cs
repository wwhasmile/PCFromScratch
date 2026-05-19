namespace PCFromScratch.DBModels;

public class MotherboardChipset
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public MotherboardChipset(string name) : this(Guid.CreateVersion7(), name)
    {
    }

    public MotherboardChipset(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private MotherboardChipset()
    {
    }
}
