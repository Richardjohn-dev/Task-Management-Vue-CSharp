namespace Domain.Tasks.Enqueue;

public record TaskFailedEventArgs(DomainTaskInfo Task, string[] Errors);

