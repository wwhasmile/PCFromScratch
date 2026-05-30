using System.ComponentModel.DataAnnotations;

namespace PCFromScratch.Common;

public enum WarningSeverity
{
    [Display(Name="Несумісність")]
    Incompatibility,
    [Display(Name = "Вузьке місце")]
    Bottleneck,
    [Display(Name = "Інфо")]
    Info //Something that is not critical, but better to mention it
}