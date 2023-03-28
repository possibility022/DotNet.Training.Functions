using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DotNet.Training.VideoService.Function
{
    public class ConvertVideo
    {
        private readonly IVideoService videoService;

        public ConvertVideo(IVideoService videoService)
        {
            this.videoService = videoService;
        }

        [FunctionName("ConvertVideo")]
        public async Task Run([QueueTrigger("convertVideoQueue", Connection = "AzureWebJobsStorage")] string videoId,
            [Queue("extractPreviewQueue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> extractPreviewQueueCollector,
            [Table("VideoState"), StorageAccount("AzureWebJobsStorage")] TableClient stateTable,
            ILogger log)
        {
            log.LogInformation("Video conversion started - {videoId}", videoId);

            var result = await videoService.ScheduleVideoConverting(videoId, 480, 240);

            await stateTable.UpsertEntityAsync(VideoState.FromState(result.ToString(), videoId, nameof(ConvertVideo)));

            log.LogInformation("Video conversion finished - {videoId}", videoId);

            extractPreviewQueueCollector.Add(videoId);
        }
    }
}
