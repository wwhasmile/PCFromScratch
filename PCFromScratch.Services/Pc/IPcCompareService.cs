using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IPcCompareService
{
    //Bool - is pc fit requirements ,key of dictionary - category name (Cpu, Ram, etc.), value - message about it when not fit
    Task<RequirementsResultDtoModel> IsFitRequirements(PcDtoModel pc, SystemRequirementsDtoModel requirements);

    Task<List<PcCompareMessage>> ComparePcs(PcDtoModel a, PcDtoModel b);
}