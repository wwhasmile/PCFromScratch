namespace PCFromScratch.Scrapers.CSVModels;

public class Ram
{
    public string? Model { get; set; } //Name
    public string? Submodel { get; set; } //Additional info that distinguishes between different versions of the same model when selecting in constructor (like DDR5-6000 CL30)
    public string? Link { get; set; }
    public string? Capacity { get; set; } //value is string with value "Amount x CapacityOfOne"
    public string? Voltage { get; set; }
    public string? Generation { get; set; }
    public string? Frequency { get; set; }
}
