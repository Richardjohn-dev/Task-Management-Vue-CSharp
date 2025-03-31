namespace Domain.Tasks.Enqueue;

public record DomainTaskQueueName(string Name)
{
    public string Number => Name[(Name.LastIndexOf('_') + 1)..];
};

public record DomainEntityDetails(SomeItemInfo Id, SharedGroupIdentifier GroupId);

public record SharedGroupIdentifier(string Value);
public record SomeItemInfo(string Value);



public record DomainTaskInfo(DomainEntityDetails Details, TaskTriggeredBy Source)
{
    public string JobId = Guid.NewGuid().ToString();
    public string CompositeKey => $"{Details.GroupId.Value}:{Details.Id.Value}";
}



public enum TaskTriggeredBy
{
    BackgroundService,
    SPA
}

