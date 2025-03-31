namespace Domain.Tasks.Enqueue;


public record DomainTaskResult(bool TaskSuccessful);

public class DomainTaskResultUI
{
    public bool TaskSuccessful { get; set; }

    public DomainTaskResultUI(DomainTaskResult taskResult)
    {
        TaskSuccessful = taskResult.TaskSuccessful;
    }
}