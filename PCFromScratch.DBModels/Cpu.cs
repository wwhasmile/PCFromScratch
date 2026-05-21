namespace PCFromScratch.DBModels;

public class Cpu
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public required Guid ManufacturerId { get; set; }
    public required Guid SeriesId { get; set; }
    public required Guid SocketId { get; set; }

    public virtual required Manufacturer Manufacturer { get; set; }
    public virtual required CpuSeries Series { get; set; }
    public virtual required CpuSocket Socket { get; set; }

    public int ProcessNode { get; set; }
    public int Cores { get; set; }
    public int Threads { get; set; }
    public float Frequency { get; set; }
    public float TurboFrequency { get; set; }
    public int L1 { get; set; }
    public int L2 { get; set; }
    public int L3 { get; set; }
    public int TDP { get; set; }
    public int PciE { get; set; }
    public int Temperature { get; set; }
    public int Score { get; set; }
    public int MaxRAM { get; set; }
    public int MaxRAMFrequency { get; set; }
    public int RAMChannels { get; set; }
}
