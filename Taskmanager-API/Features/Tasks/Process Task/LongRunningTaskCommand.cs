using Domain.Tasks.Enqueue;
using HangfireParallelTasks.Domain.Primitives;
using MediatR;

namespace Domain.Tasks.Synchronization;

public record LongRunningTaskCommand(DomainTaskInfo taskInfo) : IRequest<EndpointResponse<DomainTaskResult>>;

public class LongRunningTaskCommandHandler : IRequestHandler<LongRunningTaskCommand, EndpointResponse<DomainTaskResult>>
{
    // Example long running task
    public async Task<EndpointResponse<DomainTaskResult>> Handle(LongRunningTaskCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        return new DomainTaskResult(true);
    }
}
