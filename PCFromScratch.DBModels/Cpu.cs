using PCFromScratch.Common;

namespace PCFromScratch.DBModels;

public class Cpu
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Link { get; set; }

    public required string Socket { get; set; }
    public int Tdp { get; set; }
    public required string RamGen { get; set; }
    public int RamFrequency { get; set; }
    public CpuPacking Packing { get; set; }
    public byte[]? Image { get; set; }
    public string PriceRange { get; set; }
    public List<Offer> Offers { get; set; } = new ();
}