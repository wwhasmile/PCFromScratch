namespace PCFromScratch.Common;

public record struct PcCompareMessage(string Component, PcCompareMetric Metric, string? Text = null);