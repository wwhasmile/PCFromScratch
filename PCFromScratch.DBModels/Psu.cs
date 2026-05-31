using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class Psu
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public int Power { get; set; }
    public PsuLevel Level { get; set; }

    public ICollection<PsuConnector> Connectors { get; set; } = new HashSet<PsuConnector>();
    public required string PowerConnector { get; set; }

    public PsuFormFactor FormFactor { get; set; }
    public PsuModularity Modularity { get; set; }
}