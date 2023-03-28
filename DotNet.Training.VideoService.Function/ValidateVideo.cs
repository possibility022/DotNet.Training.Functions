using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DotNet.Training.VideoService.Function
{
    public class ValidateVideo
    {
        private readonly IVideoService videoService;

        public ValidateVideo(IVideoService videoService)
        {
            this.videoService = videoService;
        }

        [FunctionName("ValidateVideo")]
        public async Task Run([QueueTrigger("validateVideoQueue", Connection = "AzureWebJobsStorage")]string videoId, 
            [Queue("convertVideoQueue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> convertQueueCollector,
            [Table("VideoState"), StorageAccount("AzureWebJobsStorage")] TableClient stateTable,
            ILogger log)
        {
            log.LogInformation("Video validation started - {videoId}", videoId);

            var result = await videoService.ValidateVideo(videoId);

            await stateTable.UpsertEntityAsync(VideoState.FromState(result.ToString(), videoId, nameof(ValidateVideo)));

            log.LogInformation("Video validation finished - {videoId}", videoId);

            convertQueueCollector.Add(videoId);
        }
    }
}
