namespace PCFromScratch.DBModels;

public class Pin
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Pin(string name) : this(Guid.CreateVersion7(), name)
    {
    }

    public Pin(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private Pin()
    {
    }
}