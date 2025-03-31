using HangfireParallelTasks.Features.Tasks.Constants;

namespace Domain.Tasks.Enqueue;

public record TaskEventArgs(DomainTaskInfo Task, JobStatus Status);

