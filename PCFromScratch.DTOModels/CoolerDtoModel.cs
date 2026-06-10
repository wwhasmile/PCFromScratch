using PCFromScratch.Common;

namespace PCFromScratch.DTOModels;

public record struct CoolerDtoModel(Guid Id, string Name, string Link, int Tdp, int FanCount, int Radius, int Thickness, int Speed,
        int Height, CoolerType Type, string? ImageUrl, int MinPrice, int MaxPrice);