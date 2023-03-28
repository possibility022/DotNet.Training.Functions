using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace DotNet.Training.VideoService.Function.Durable.Activities;

public class VideoActivities
{
    private readonly IVideoService videoService;

    public VideoActivities(IVideoService videoService)
    {
        this.videoService = videoService;
    }

    [FunctionName(nameof(ActivityStartVideoValidation))]
    public async Task<bool> ActivityStartVideoValidation([ActivityTrigger] string videoId)
    {
        var videoIsValid = await videoService.ValidateVideo(videoId);
        return videoIsValid;
    }

    [FunctionName(nameof(ActivityExtractIcon))]
    public async Task<string> ActivityExtractIcon([ActivityTrigger] string videoId)
    {
        var videoIconUri = await videoService.ExtractIcon(videoId);
        return videoIconUri;
    }

    [FunctionName(nameof(ActivityConvertToAllResolutions))]
    public Task<string[]> ActivityConvertToAllResolutions([ActivityTrigger] string videoId)
    {
        var hdTask = videoService.ScheduleVideoConverting(videoId, 1920, 1024);
        var hqTask = videoService.ScheduleVideoConverting(videoId, 1366, 768);

        return Task.WhenAll(hdTask, hqTask);
    }

    [FunctionName(nameof(ActivityExtractPreview))]
    public async Task<string> ActivityExtractPreview([ActivityTrigger] string videoId)
    {
        var videoIsValid = await videoService.ExtractPreview(videoId);
        return videoIsValid;
    }
}
