namespace DoNet.Training.NotificationClient;

public interface INotificationService
{
    Task SendNotification(string content);
}