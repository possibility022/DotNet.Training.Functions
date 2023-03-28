using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DotNet.Training.VideoService.Function.Durable.Activities;
using DotNet.Training.VideoService.Function.Durable.Models;

namespace DotNet.Training.VideoService.Function.Durable.Durable;

public class VideoProcessingOrchestration
{
    private readonly ILogger<VideoProcessingOrchestration> logger;

    public VideoProcessingOrchestration(ILogger<VideoProcessingOrchestration> logger)
    {
        this.logger = logger;
    }

    [FunctionName(nameof(VideoProcessingOrchestration))]
    public async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var input = context.GetInput<VideoProcessingInput>();

        var isValid = await context.CallActivityAsync<bool>(nameof(VideoActivities.ActivityStartVideoValidation), input.VideoId);

        if (!isValid)
        {
            await context.CallActivityAsync(nameof(NotificationActivities.ActivitySendNotification),
                "Video is not valid ... id and more info here.");
            return;
        }

        var convertVideoTask = context.CallActivityAsync<string[]>(nameof(VideoActivities.ActivityConvertToAllResolutions), input.VideoId);
        var extractIconTask = context.CallActivityAsync<string>(nameof(VideoActivities.ActivityExtractIcon), input.VideoId);
        var extractPreviewTask = context.CallActivityAsync<string>(nameof(VideoActivities.ActivityExtractPreview), input.VideoId);

        await Task.WhenAll(convertVideoTask, extractIconTask, extractPreviewTask);

        var resultsModel = new VideoProcessingResults()
        {
            IconUri = extractIconTask.Result,
            PreviewUri = extractPreviewTask.Result,
            VideoUris = convertVideoTask.Result,
        };

        await context.CallActivityAsync(nameof(NotificationActivities.ActivitySendNotification), resultsModel);

        context.SetOutput(resultsModel);
    }
}
