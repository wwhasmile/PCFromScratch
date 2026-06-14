namespace PCFromScratch.DTOModels;

public record struct RequirementsResultDtoModel(bool IsFit, Dictionary<string, string> Messages);