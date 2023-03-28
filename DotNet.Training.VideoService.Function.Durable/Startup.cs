using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using DoNet.Training.NotificationClient;


[assembly: FunctionsStartup(typeof(DotNet.Training.VideoService.Function.Durable.Startup))]

namespace DotNet.Training.VideoService.Function.Durable;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Services.AddSingleton<IVideoService, VideoService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
    }
}
