using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DotNet.Training.VideoService.Function
{
    public class ExtractPreview
    {
        private readonly IVideoService videoService;

        public ExtractPreview(IVideoService videoService)
        {
            this.videoService = videoService;
        }

        [FunctionName("ExtractPreview")]
        public async Task Run([QueueTrigger("extractPreviewQueue", Connection = "AzureWebJobsStorage")] string videoId,
            [Queue("sendNotificationQueue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> sendNotificationQueueCollector,
            [Table("VideoState"), StorageAccount("AzureWebJobsStorage")] TableClient stateTable,
            ILogger log)
        {
            log.LogInformation("Video preview extract started - {videoId}", videoId);

            var result = await videoService.ExtractPreview(videoId);

            await stateTable.UpsertEntityAsync(VideoState.FromState(result.ToString(), videoId, nameof(ExtractPreview)));

            sendNotificationQueueCollector.Add(videoId);

            log.LogInformation("Video preview extract finished - {videoId}", videoId);
        }
    }
}
