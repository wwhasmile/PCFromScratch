using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for comparing PC configurations and checking against system requirements.
/// </summary>
public interface IPcCompareService
{
    /// <summary>
    /// Checks if a given PC configuration meets specific system requirements.
    /// </summary>
    /// <param name="pc">The PC configuration to check.</param>
    /// <param name="requirements">The system requirements to check against.</param>
    /// <returns>A result model containing a boolean indicating if requirements are met, and a dictionary of category messages (e.g., Cpu, Ram) explaining any shortcomings.</returns>
    Task<RequirementsResultDtoModel> IsFitRequirements(PcDtoModel pc, SystemRequirementsDtoModel requirements);

    /// <summary>
    /// Compares two PC configurations and provides a detailed breakdown of the differences.
    /// </summary>
    /// <param name="a">The first PC configuration.</param>
    /// <param name="b">The second PC configuration.</param>
    /// <returns>A list of messages detailing the comparison results.</returns>
    Task<List<PcCompareMessage>> ComparePcs(PcDtoModel a, PcDtoModel b);
}