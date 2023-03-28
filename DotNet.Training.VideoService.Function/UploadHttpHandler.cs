using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DotNet.Training.VideoService.Function
{
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
            [Queue("validateVideoQueue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> validateQueueCollector,
            [Table("VideoState"), StorageAccount("AzureWebJobsStorage")] TableClient stateTable,
            ILogger log)
        {
            var videoId = Guid.NewGuid().ToString();

            log.LogInformation("Starting video storing function - videoId {videoId}", videoId);

            await videoService.StoreVideo(videoId, req.Body);

            await stateTable.UpsertEntityAsync(VideoState.FromState("STORED", videoId, nameof(UploadHttpHandler)));

            log.LogInformation("Video stored - queueing - videoId {videoId}", videoId);

            validateQueueCollector.Add(videoId);

            return new OkObjectResult($"Your video was send and is under processing - videoId: {videoId}");
        }
    }
}
