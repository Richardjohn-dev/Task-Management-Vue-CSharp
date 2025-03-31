using Databridge.BackgroundServices.Synchronization;
using Domain.Tasks.Enqueue;
using Domain.Tasks.Synchronization;
using Hangfire;
using Hangfire.MemoryStorage;

namespace HangfireParallelTasks.Services.Registration;

public static class ServicesRegistration
{

    public static IServiceCollection RegisterTaskManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire((sp, config) =>
        {
            config.UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMemoryStorage(); // In memory for demo purpose.. use sql/db storage if persisting tasks.

        });

        var queueCount = configuration.GetValue<int>("Hangfire:ParallelQueueCount", 3); // 


        var synchronizationQueueNames = Enumerable.Range(1, queueCount)
                                    .Select(i => $"synchronization_queue_{i}").ToArray();


        List<string> queueNames = new List<string>(synchronizationQueueNames)
        {
            "another_task_queue", "different_task_queue"
        };

        services.AddHangfireServer(options =>
        {
            options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
            options.Queues = queueNames.ToArray();
            options.WorkerCount = synchronizationQueueNames.Count();

        });



        services.AddSingleton(sp => new DomainTaskQueue(synchronizationQueueNames)); // queue names registered in queue.
        services.AddScoped<DomainTaskProcessor>();

        services.AddHostedService<EnqueueDomainTasksBackgroundService>();
        services.AddScoped<EnqueueDomainTasksForBackgroundService>();

        return services;

    }
}
