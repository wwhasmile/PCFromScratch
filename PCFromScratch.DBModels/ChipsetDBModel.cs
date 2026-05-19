namespace PCFromScratch.DBModels;

public class ChipsetDBModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ChipsetDBModel(string name) : this(Guid.CreateVersion7(), name)
    {
    }

    public ChipsetDBModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private ChipsetDBModel()
    {
    }
}
