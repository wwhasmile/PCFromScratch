using PCFromScratch.Common;

namespace PCFromScratch.DTOModels;

public record struct MotherboardDtoModel(Guid Id, string Name, string Socket, MotherboardFormFactor FormFactor,
        string Chipset, string RamGeneration, int RamSlots, int RamFrequency, bool HasM2Slot,
        string? ImageUrl, int MinPrice, int MaxPrice);