using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class Cooler
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Link { get; set; }

    public int Tdp { get; set; }
    public List<string> IntelSockets { get; set; } = new ();
    public List<string> AmdSockets { get; set; } = new ();

    public int FanCount { get; set; }
    public int Radius { get; set; }
    public int Thickness { get; set; }
    public int Speed { get; set; }
    
    public int Height { get; set; }

    public CoolerType Type { get; set; }
    public string? ImageUrl { get; set; }
    public int MinPrice { get; set; }
    public int MaxPrice { get; set; }
    public HashSet<Offer> Offers { get; set; } = new ();
}