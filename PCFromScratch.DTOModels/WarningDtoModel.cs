using PCFromScratch.Common;

namespace PCFromScratch.DTOModels;

public record struct WarningDtoModel(EnumWithName<WarningSeverity> Severity, string Message);