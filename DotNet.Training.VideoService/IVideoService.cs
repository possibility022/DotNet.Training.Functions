namespace DotNet.Training.VideoService;

public interface IVideoService
{
    Task<string> ExtractIcon(string videoId);
    Task<string> ExtractPreview(string videoId);
    Task<string> ScheduleVideoConverting(string videoId, int resolutionX, int resolutionY);
    Task StoreVideo(string videoId, Stream stream);
    Task<bool> ValidateVideo(string videoId);
}