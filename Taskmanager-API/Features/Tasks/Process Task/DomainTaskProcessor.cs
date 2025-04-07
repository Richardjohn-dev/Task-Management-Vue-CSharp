using Domain.Tasks.Enqueue;
using Hangfire;
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
            await NotifyTaskResults(result, taskInfo);

        }
        catch (Exception ex)
        {
            if (TriggeredFromUI(taskInfo))
                await NotifyFailedTask(taskInfo, ex.Message);
        }
        finally
        {
            await _queueService.OnTaskComplete(queue, taskInfo);
        }
    }

    private async Task NotifyTaskResults(Result<DomainTaskResultUI> result, DomainTaskInfo taskInfo)
    {
        if (IsFailed(result))
            await NotifyFailedTask(taskInfo, result.Errors.ToArray());

        await _queueService.SendSynchonizationResults(result);
    }

    private async Task NotifyFailedTask(DomainTaskInfo taskInfo, params string[] errors)
     => await _queueService.NotifySynchronizationTaskFailed(taskInfo, errors);



    private void LogQueueStarted(DomainTaskQueueName queueName, DomainTaskInfo taskInfo)
    {
        Console.WriteLine();
        Console.WriteLine($"******> PROCESSING: QUEUE#{queueName.Number} Task Started ({taskInfo.Details.Group.Id})");
        Console.WriteLine();
    }
    private async Task<Result<DomainTaskResultUI>> DoWork(DomainTaskInfo taskInfo)
    {

        await _queueService.NotifySynchronizationTaskStarted(taskInfo); // notify frontend


        Stopwatch taskTimer = new();
        taskTimer.Start();

        // example long running task here. 
        var taskResult = await _mediatr.Send(new LongRunningTaskCommand(taskInfo));

        taskTimer.Stop();

        return taskResult.IsSuccess ? new DomainTaskResultUI(taskResult.Value, taskInfo)
                   : Result.Error(taskResult.Errors.ToArray());

    }


    private static bool TriggeredFromUI(DomainTaskInfo taskInfo) => taskInfo.Source == TaskTriggeredBy.SPA;


    private static bool IsFailed(Result<DomainTaskResultUI> result)
        => (result.IsSuccess == false || result.Value == null);

}



