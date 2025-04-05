using HangfireParallelTasks.Features.Tasks.Constants;

namespace Domain.Tasks.Enqueue;

public static class TaskHelper
{

    public static DomainTaskInfo NewTaskFromEndpoint(DomainEntityDetails Details)
        => new DomainTaskInfo(Details, TaskTriggeredBy.SPA);

    public static DomainTaskInfo NewTaskFromBackgroundService(DomainEntityDetails Details)
     => new DomainTaskInfo(Details, TaskTriggeredBy.BackgroundService);

    public static TaskStatusUpdateResponse Enqueued(this DomainTaskInfo task)
        => new(task.Details.TaskKey, DomainTaskStatus.Enqueued);
    public static TaskStatusUpdateResponse Processing(this DomainTaskInfo task)
        => new(task.Details.TaskKey, DomainTaskStatus.Processing);
}