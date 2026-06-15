using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

/// <summary>
/// Defines a service for checking PC component compatibility.
/// </summary>
public interface IPcCheckService
{
    /// <summary>
    /// Checks the compatibility of components in a given PC configuration.
    /// </summary>
    /// <param name="pc">The PC configuration to check.</param>
    /// <returns>A list of warnings regarding compatibility issues.</returns>
    Task<List<Warning>> CheckPc(PcDtoModel pc);
}