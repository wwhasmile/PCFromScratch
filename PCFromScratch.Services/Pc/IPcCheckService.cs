using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IPcCheckService
{
    Task<List<Warning>> CheckPc(PcDtoModel pc);
}