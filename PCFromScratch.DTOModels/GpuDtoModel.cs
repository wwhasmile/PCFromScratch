namespace PCFromScratch.DTOModels;

public record struct GpuDtoModel(Guid Id, string Name, int Length, string? Image, int MinPrice, int MaxPrice);