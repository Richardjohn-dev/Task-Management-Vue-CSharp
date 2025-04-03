
using Domain.Tasks.Enqueue;
using FastEndpoints;
using HangfireParallelTasks.Domain.Primitives;

public record EnqueueTaskRequest(DomainEntityDetails Details);


public class EnqueueDomainTaskEndpoint : Endpoint<EnqueueTaskRequest, ApiResponse<TaskEnqueuedResponse>>
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
            await SendProblemDetails(ct, "invalid entry data");
            return;
        }

        var taskToEnqueue = new DomainTaskInfo(req.Details, TaskTriggeredBy.SPA);
        var enqueueTaskResponse = await _queue.TryEnqueueDomainTask(taskToEnqueue);

        if (!enqueueTaskResponse.IsSuccess)
        {
            await SendProblemDetails(ct, enqueueTaskResponse.Errors.ToArray());
            return;
        }
        var response = ApiResponse<TaskEnqueuedResponse>.Success(enqueueTaskResponse);

        await SendAsync(response, cancellation: ct);
    }

    private async Task SendProblemDetails(CancellationToken ct, params string[] errors)
    {
        foreach (var error in errors)
            AddError(error);

        await SendErrorsAsync(statusCode: StatusCodes.Status400BadRequest, cancellation: ct);
        return;
    }

    private static bool RequestInvalid(EnqueueTaskRequest req) => req.Details is null || SampleData.DetailsExist(req.Details) == false;
}

