
using Domain.Tasks.Enqueue;

public class TaskEnqueuedResponse()
{
    public string Key { get; protected set; }
    public string Message { get; set; }
    public static TaskEnqueuedResponse Enqueued(DomainTaskInfo identifier)
    {
        return new TaskEnqueuedResponse
        {
            Key = identifier.CompositeKey,
            Message = "Mapping Task enqueued"
        };

    }

}
