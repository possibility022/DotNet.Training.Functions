using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DotNet.Training.VideoService.Function.Durable.Durable;
using DotNet.Training.VideoService.Function.Durable.Models;

namespace DotNet.Training.VideoService.Function.Durable.Triggers;

public class UploadHttpHandler
{
    private readonly IVideoService videoService;

    public UploadHttpHandler(IVideoService videoService)
    {
        this.videoService = videoService;
    }

    [FunctionName("UploadHttpHandler")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [DurableClient] IDurableClient durableClient,
        ILogger log)
    {

        var context = new VideoProcessingInput()
        {
            VideoId = Guid.NewGuid().ToString(),
        };

        await videoService.StoreVideo(context.VideoId, req.Body);

        var orchestrationId = await durableClient.StartNewAsync<VideoProcessingInput>(nameof(VideoProcessingOrchestration), context);

        return new OkObjectResult(orchestrationId);
    }
}
