namespace PCFromScratch.DBModels;

public class ManufacturerDBModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public ManufacturerDBModel()
    {
    }

    public ManufacturerDBModel(string name) : this(Guid.NewGuid(), name)
    {
    }

    public ManufacturerDBModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
