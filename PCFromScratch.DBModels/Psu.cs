using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class Psu
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public int Power { get; set; }
    public PsuRating Rating { get; set; }

    public FormFactor FormFactor { get; set; }
}