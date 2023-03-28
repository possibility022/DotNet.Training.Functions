namespace DoNet.Training.NotificationClient;

public class NotificationService : INotificationService
{
    public async Task SendNotification(string content)
    {
        await Task.Delay(1000);
    }
}