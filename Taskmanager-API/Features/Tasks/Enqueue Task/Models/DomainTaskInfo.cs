namespace Domain.Tasks.Enqueue;

public record DomainTaskQueueName(string Name)
{
    public string Number => Name[(Name.LastIndexOf('_') + 1)..];
};



public record GroupItems(SharedGroupIdentifier Group, GroupItem[] Items);

public record DomainEntityDetails(GroupItem Item, SharedGroupIdentifier Group)
{
    public TaskKey TaskKey => new($"{Group.Id}:{Item.Id}");
};

public record SharedGroupIdentifier(string Id);
public record GroupItem(string Id);



public record DomainTaskInfo(DomainEntityDetails Details, TaskTriggeredBy Source);




public enum TaskTriggeredBy
{
    BackgroundService,
    SPA
}


public record TaskKey(string Value);


