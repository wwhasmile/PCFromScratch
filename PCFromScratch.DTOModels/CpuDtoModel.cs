using PCFromScratch.Common;

namespace PCFromScratch.DTOModels;

public record struct CpuDtoModel(Guid Id, string Name, string Link, string Socket, int Tdp, string RamGen,
        int RamFrequency, CpuPacking Packaging, string? Image, int MinPrice, int MaxPrice);