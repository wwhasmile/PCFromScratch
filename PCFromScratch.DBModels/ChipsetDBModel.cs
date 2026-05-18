namespace PCFromScratch.DBModels;

public class ChipsetDBModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public ChipsetDBModel()
    {
    }

    public ChipsetDBModel(string name) : this(Guid.NewGuid(), name)
    {
    }

    public ChipsetDBModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
