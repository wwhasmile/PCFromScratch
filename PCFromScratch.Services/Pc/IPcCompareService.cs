using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IPcCompareService
{
    //Bool - is pc fit requirements ,key of dictionary - category name (Cpu, Ram, etc.), value - message about it when not fit
    Task<(bool, Dictionary<string, string>)> IsFitRequirements(PcDtoModel pc, SystemRequirementsDtoModel requirements);

    IAsyncEnumerable<PcCompareMessage> ComparePcs(PcDtoModel a, PcDtoModel b);
}