namespace PCFromScratch.DTOModels;

public record struct GpuDtoModel(Guid Id, string Name, int Tdp, int Length, string? Image, int MinPrice, int MaxPrice);