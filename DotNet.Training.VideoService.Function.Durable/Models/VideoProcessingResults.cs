namespace DotNet.Training.VideoService.Function.Durable.Models;
public record VideoProcessingResults
{
    public string IconUri { get; init; }
    public string PreviewUri { get; init; }
    public string[] VideoUris { get; init; }
}
