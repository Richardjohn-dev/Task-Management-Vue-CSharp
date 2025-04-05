using System.Text.Json.Serialization;

namespace HangfireParallelTasks.Features.Tasks.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DomainTaskStatus
{
    Enqueued,
    Processing,
    Completed,
    Failed
}
