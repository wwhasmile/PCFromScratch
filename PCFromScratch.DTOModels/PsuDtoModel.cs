using PCFromScratch.Common;

namespace PCFromScratch.DTOModels;

public record struct PsuDtoModel(Guid Id, string Name, string Link, int Power, PsuLevel Level, string PowerConnector,
        PsuFormFactor FormFactor, PsuModularity Modularity, string? Image, int MinPrice, int MaxPrice);