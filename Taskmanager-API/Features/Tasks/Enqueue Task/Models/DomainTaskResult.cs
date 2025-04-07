namespace Domain.Tasks.Enqueue;


public record DomainTaskResult(bool TaskSuccessful);

public class DomainTaskResultUI
{
    public bool TaskSuccessful { get; set; }
    public DomainTaskInfo DomainTaskInfo { get; set; }

    public DomainTaskResultUI(DomainTaskResult taskResult, DomainTaskInfo taskInfo)
    {
        TaskSuccessful = taskResult.TaskSuccessful;
        DomainTaskInfo = taskInfo;
    }
}