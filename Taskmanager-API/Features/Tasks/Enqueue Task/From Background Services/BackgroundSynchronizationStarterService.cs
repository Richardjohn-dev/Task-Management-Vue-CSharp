
using Databridge.BackgroundServices.Synchronization;

public class EnqueueDomainTasksBackgroundService : BackgroundService
{
    public EnqueueDomainTasksBackgroundService(IServiceProvider services)
    {
        Services = services;
    }

    public IServiceProvider Services { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunBackgroundService(stoppingToken);
    }

    private async Task RunBackgroundService(CancellationToken stoppingToken)
    {
        using var scope = Services.CreateScope();
        var scopedProcessingService =
                scope.ServiceProvider.GetRequiredService<EnqueueDomainTasksForBackgroundService>();

        await scopedProcessingService.RunSynchronizationService(stoppingToken);
    }
}




