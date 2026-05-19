namespace PCFromScratch.DBModels;

public class ManufacturerDBModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ManufacturerDBModel(string name) : this(Guid.CreateVersion7(), name)
    {
    }

    public ManufacturerDBModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private ManufacturerDBModel()
    {
    }
}
