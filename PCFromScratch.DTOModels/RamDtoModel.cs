namespace PCFromScratch.DTOModels;

public record struct RamDtoModel(Guid Id, string Model, string? Submodel, int Amount, int Sticks, float Voltage,
        string Generation, int Frequency, string? Image, int MinPrice, int MaxPrice);