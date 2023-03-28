namespace DotNet.Training.VideoService;

public class VideoService : IVideoService
{
    public async Task StoreVideo(string videoId, Stream stream)
    {
        await Task.Delay(500);
    }

    public async Task<bool> ValidateVideo(string videoId)
    {
        await Task.Delay(500);
        return true;
    }

    public async Task<string> ScheduleVideoConverting(string videoId, int resolutionX, int resolutionY)
    {
        await Task.Delay(500);
        return $"taskIdentifier\\videoConverting\\{videoId}_{resolutionX}x{resolutionY}";
    }

    public async Task<string> ExtractIcon(string videoId)
    {
        await Task.Delay(500);
        return "UriTo_Icon\\" + videoId;
    }

    public async Task<string> ExtractPreview(string videoId)
    {
        await Task.Delay(500);
        return "UriTo_Preview\\" + videoId;
    }
}