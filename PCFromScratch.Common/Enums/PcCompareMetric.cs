using System.ComponentModel.DataAnnotations;

namespace PCFromScratch.Common;

public enum PcCompareMetric
{
    [Display(Name = "<")]
    Worse = -1,
    [Display(Name = "=")]
    Equal = 0,
    [Display(Name = ">")]
    Better = 1,
}