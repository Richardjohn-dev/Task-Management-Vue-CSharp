
using Domain.Tasks.Enqueue;

public class EnqueueDomainTaskResponse()
{
    public string Key { get; protected set; }
    public string Message { get; set; }
    public static EnqueueDomainTaskResponse Enqueued(DomainTaskInfo identifier)
    {
        return new EnqueueDomainTaskResponse
        {
            Key = identifier.CompositeKey,
            Message = "Mapping Task enqueued"
        };

    }

}

