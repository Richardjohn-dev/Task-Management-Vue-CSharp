using Domain.Tasks.Enqueue;
using Hangfire;
using HangfireParallelTasks.Domain.Primitives;
using MediatR;
using System.Diagnostics;

namespace Domain.Tasks.Synchronization;


public class DomainTaskProcessor
{
    private readonly IMediator _mediatr;
    private readonly DomainTaskQueue _queueService;
    //private readonly ApplicationDbContext _context; // Scoped per task within hangfire

    public DomainTaskProcessor(IMediator mediatr, DomainTaskQueue queue)
    {
        _mediatr = mediatr;
        _queueService = queue;
    }


    [Queue("{0}")]
    public async Task TryProcessSynchronizationTask(string queueName, DomainTaskQueueName queue, DomainTaskInfo taskInfo)
    {
        LogQueueStarted(queue, taskInfo);

        try
        {
            var result = await DoWork(taskInfo);
            NotifyTaskResults(result, taskInfo);

        }
        catch (Exception ex)
        {
            if (TriggeredFromUI(taskInfo))
                NotifyFailedTask(taskInfo, ex.Message);
        }
        finally
        {
            await _queueService.OnTaskComplete(queue, taskInfo);
        }
    }

    private void NotifyTaskResults(EndpointResponse<DomainTaskResultUI> result, DomainTaskInfo taskInfo)
    {
        if (IsFailed(result))
            NotifyFailedTask(taskInfo, result.Errors.ToArray());

        _queueService.SendSynchonizationResults(result);
    }

    private void NotifyFailedTask(DomainTaskInfo taskInfo, params string[] errors)
     => _queueService.NotifySynchronizationTaskFailed(taskInfo, errors);



    private void LogQueueStarted(DomainTaskQueueName queueName, DomainTaskInfo taskInfo)
    {
        Console.WriteLine();
        Console.WriteLine($"******> PROCESSING: QUEUE#{queueName.Number} Task Started ({taskInfo.Details.GroupId.Value})");
        Console.WriteLine();
    }
    private async Task<EndpointResponse<DomainTaskResultUI>> DoWork(DomainTaskInfo taskInfo)
    {
        _queueService.NotifySynchronizationTaskStarted(taskInfo); // notify frontend

        Stopwatch taskTimer = new();
        taskTimer.Start();

        // example long running task here. 
        var taskResult = await _mediatr.Send(new LongRunningTaskCommand(taskInfo));

        taskTimer.Stop();

        return taskResult.Success ? new DomainTaskResultUI(taskResult.Payload)
                   : Result.Error(taskResult.Errors.ToArray());

    }


    private static bool TriggeredFromUI(DomainTaskInfo taskInfo) => taskInfo.Source == TaskTriggeredBy.SPA;


    private static bool IsFailed(EndpointResponse<DomainTaskResultUI> result)
        => (result.Success == false || result.Payload == null);

}



