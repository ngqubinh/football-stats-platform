namespace FSP.Domain.Entities.Core;

public class URLInformation
{
    public string Label { get; set; } = string.Empty;
    public string URL { get; set; } = string.Empty;
    public League League { get; set; } = new League();
    public int StatusCode { get; set; }
    public string Status { get; set; } = string.Empty;

    public string Season { get; set; } = string.Empty;
}
