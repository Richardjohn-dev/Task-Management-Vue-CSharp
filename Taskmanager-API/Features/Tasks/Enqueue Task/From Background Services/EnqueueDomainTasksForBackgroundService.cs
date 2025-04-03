using Domain.Tasks.Enqueue;
using HangfireParallelTasks.Domain.Primitives;
using System.Globalization;

namespace Databridge.BackgroundServices.Synchronization;


internal class EnqueueDomainTasksForBackgroundService
{

    private readonly DomainTaskQueue _queue;
    private readonly ILogger<EnqueueDomainTasksForBackgroundService> _logger;
    public EnqueueDomainTasksForBackgroundService(DomainTaskQueue queue, ILogger<EnqueueDomainTasksForBackgroundService> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public async Task RunSynchronizationService(CancellationToken ct)
    {

        var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(ct);

                ConsoleLog(GetTimeNow());

                var allTasks = SampleData.GetAll.Select(details => new DomainTaskInfo(details, TaskTriggeredBy.BackgroundService)).ToArray();

                Result tasksLoaded2 = await _queue.TryEnqueueTasksFromBackgroundService(allTasks);

                ConsoleLog(GetTimeNow(), true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in token rotation service");
            }
        }
    }



    private static string GetTimeNow()
        => DateTime.UtcNow.ToString("dddd, dd MMMM yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);


    private static void ConsoleLog(string time, bool finished = false)
    {
        if (finished)
        {
            Console.WriteLine($"** Finished Enqueing Tasks at {time} UTC");
        }
        else
        {
            Console.WriteLine($"** Enqueuing Tasks at {time} UTC");
        }
    }
}


