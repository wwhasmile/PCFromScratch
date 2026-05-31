using System.ComponentModel.DataAnnotations;

namespace PCFromScratch.Common;

public enum PsuLevel
{
    [Display(Name = "80+ Standard")]
    Standard,
    [Display(Name = "80+ Bronze")]
    Bronze,
    [Display(Name = "80+ Silver")]
    Silver,
    [Display(Name = "80+ Gold")]
    Gold,
    [Display(Name = "80+ Platinum")]
    Platinum,
    [Display(Name = "80+ Titanium")]
    Titanium
}