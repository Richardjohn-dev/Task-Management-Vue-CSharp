using Databridge.BackgroundServices.Synchronization;
using Domain.Tasks.Enqueue;
using Domain.Tasks.Synchronization;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.SignalR;
using TaskManager.SignalR;

namespace HangfireParallelTasks.Services.Registration;

public static class ServicesRegistration
{
    public static void ConfigureSPACors(this IServiceCollection services, IConfiguration configuration)
    {
        //var allowedOrigins = configuration.GetSection("AppSettings:CORS-Settings:Allow-Origins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddPolicy("VueSPA",
               builder =>
               builder.WithOrigins(["http://localhost:3000", "http://localhost:3001"])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
    }
    public static IServiceCollection RegisterTaskManager(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddHangfire((sp, config) =>
        {
            config.UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMemoryStorage(); // In memory for demo purpose.. use sql/db storage if persisting tasks.

        });

        var parallelTasksQueueCount = configuration.GetValue<int>("Hangfire:ParallelQueueCount", 3);


        var synchronizationQueueNames = Enumerable.Range(1, parallelTasksQueueCount)
                                    .Select(i => $"parallel_task_queue_{i}").ToArray();


        List<string> queueNames = new List<string>(synchronizationQueueNames)
        {
            "another_task_queue", "different_task_queue"
        };

        // you could configure a server per queue, but this is quite resource intensive.
        services.AddHangfireServer(options =>
        {
            options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
            options.Queues = queueNames.ToArray();
            options.WorkerCount = queueNames.Count();
        });



        services.AddSingleton(x =>
            new DomainTaskQueue(
                x.GetRequiredService<IHubContext<TaskHub>>(),
                synchronizationQueueNames));        // register queuenames.

        services.AddScoped<DomainTaskProcessor>();

        services.AddHostedService<EnqueueDomainTasksBackgroundService>();
        services.AddScoped<EnqueueDomainTasksForBackgroundService>();

        return services;

    }


}
