using System;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using DoNet.Training.NotificationClient;

namespace DotNet.Training.VideoService.Function
{
    public class SendNotification
    {
        private readonly INotificationService notificationService;

        public SendNotification(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [FunctionName("SendNotification")]
        public async Task Run([QueueTrigger("sendNotificationQueue", Connection = "AzureWebJobsStorage")]string videoId,
            [Table("VideoState", Connection = "AzureWebJobsStorage")] TableClient tableClient,
            ILogger log)
        {
            log.LogInformation($"Sending notification for: {videoId}");

            bool sendNotification = false;

            AsyncPageable<VideoState> queryResults = tableClient.QueryAsync<VideoState>(v => v.PartitionKey == videoId);
            await foreach (VideoState entity in queryResults)
            {
                log.LogInformation($"{entity.PartitionKey}\t{entity.RowKey}\t{entity.Timestamp}\t{entity.OutputValue}");

                if (entity.RowKey == nameof(ExtractPreview))
                {
                    sendNotification = true;
                }
            }

            if (sendNotification)
            {
                await notificationService.SendNotification($"Video {videoId} was processed");
                log.LogInformation("Video {videoId} was processed successfully", videoId);
            }
            else
            {
                log.LogError("Video {videoId} was not processed successfully", videoId);
            }
        }
    }
}
