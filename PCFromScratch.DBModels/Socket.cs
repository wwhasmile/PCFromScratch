namespace PCFromScratch.DBModels;

public class Socket
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Socket(string name) : this(Guid.NewGuid(), name)
    {
    }

    public Socket(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private Socket()
    {
    }
}
