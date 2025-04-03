using Domain.Tasks.Enqueue;
using MediatR;

namespace Domain.Tasks.Synchronization;

public record LongRunningTaskCommand(DomainTaskInfo taskInfo) : IRequest<Result<DomainTaskResult>>;

public class LongRunningTaskCommandHandler : IRequestHandler<LongRunningTaskCommand, Result<DomainTaskResult>>
{
    // Example long running task
    public async Task<Result<DomainTaskResult>> Handle(LongRunningTaskCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        return new DomainTaskResult(true);
    }
}
