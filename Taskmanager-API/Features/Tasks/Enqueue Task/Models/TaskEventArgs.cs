using HangfireParallelTasks.Features.Tasks.Constants;

namespace Domain.Tasks.Enqueue;

public record TaskEventArgs(DomainTaskInfo Task, HangfireParallelTasks.Features.Tasks.Constants.DomainTaskStatus Status);

