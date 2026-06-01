using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface IPcCheckService
{
    List<Warning> CheckPc(PcDtoModel pc);
}