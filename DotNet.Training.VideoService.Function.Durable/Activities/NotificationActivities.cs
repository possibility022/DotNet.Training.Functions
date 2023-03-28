using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using DoNet.Training.NotificationClient;
using DotNet.Training.VideoService.Function.Durable.Models;

namespace DotNet.Training.VideoService.Function.Durable.Activities;
internal class NotificationActivities
{
    private readonly INotificationService notificationService;

    public NotificationActivities(INotificationService notificationService)
    {
        this.notificationService = notificationService;
    }

    [FunctionName(nameof(ActivitySendNotification))]
    public async Task ActivitySendNotification([ActivityTrigger] string message)
    {
        await notificationService.SendNotification(message);
    }

    [FunctionName(nameof(ActivitySendTaskCompletedNotification))]
    public async Task ActivitySendTaskCompletedNotification([ActivityTrigger] VideoProcessingResults results)
    {
        string message = $"Your video details are available here:\n " +
            $"Icon - {results.IconUri}\n " +
            $"Preview - {results.PreviewUri}\n " +
            $"Videos - {string.Join(", ", results.VideoUris)}";

        await notificationService.SendNotification(message);
    }
}
