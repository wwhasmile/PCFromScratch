namespace PCFromScratch.DTOModels;

public record struct InternalDriveDtoModel(Guid Id, string Name, int Capacity, string Type, string Port,
        string? Image, int MinPrice, int MaxPrice);