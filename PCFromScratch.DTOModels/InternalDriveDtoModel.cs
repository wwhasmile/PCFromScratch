namespace PCFromScratch.DTOModels;

public record struct InternalDriveDtoModel(Guid Id, string Name, string Link, int Capacity, string Type, string Port,
        string? Image, int MinPrice, int MaxPrice);