namespace PCFromScratch.DBModels;

public class SocketDBModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public SocketDBModel()
    {
    }

    public SocketDBModel(string name) : this(Guid.NewGuid(), name)
    {
    }

    public SocketDBModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
