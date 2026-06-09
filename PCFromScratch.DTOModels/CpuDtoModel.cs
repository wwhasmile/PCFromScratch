namespace PCFromScratch.DTOModels;

public record struct CpuDtoModel(Guid Id, string Name, string Socket, int Tdp, string RamGen, int RamFrequency,
        string Packaging, string? Image, int MinPrice, int MaxPrice);