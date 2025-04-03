
using Domain.Tasks.Enqueue;
using FastEndpoints;
using HangfireParallelTasks.Domain.Primitives;

public record EnqueueTaskRequest(DomainEntityDetails Details);


public class EnqueueDomainTaskEndpoint : Endpoint<EnqueueTaskRequest, EndpointResponse<TaskEnqueuedResponse>>
{
    private readonly DomainTaskQueue _queue;

    public EnqueueDomainTaskEndpoint(DomainTaskQueue queue)
    {
        _queue = queue;
    }

    public override void Configure()
    {
        Post("/api/tasks/enqueue");
        AllowAnonymous();

    }

    public override async Task HandleAsync(EnqueueTaskRequest req, CancellationToken ct)
    {
        if (RequestInvalid(req))
        {
            await SendAsync(Result.Error("invalid data"));
        }
        else
        {
            var taskToEnqueue = new DomainTaskInfo(req.Details, TaskTriggeredBy.SPA);
            var enqueueTaskResponse = await _queue.TryEnqueueDomainTask(taskToEnqueue);
            await SendAsync(enqueueTaskResponse, cancellation: ct);
        }
    }

    private static bool RequestInvalid(EnqueueTaskRequest req) => req.Details is null || SampleData.DetailsExist(req.Details) == false;
}

