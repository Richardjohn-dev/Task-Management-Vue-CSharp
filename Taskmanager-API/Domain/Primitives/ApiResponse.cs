namespace HangfireParallelTasks.Domain.Primitives;
public class ApiResponse<T>
{
    public T? Payload { get; set; }
    public Pagination? Pagination { get; private set; }

    public static ApiResponse<T> Success(T payload) =>
        new ApiResponse<T>
        {
            Payload = payload,
        };

    public static ApiResponse<T> PagedResponse(T payload, Pagination pagination) =>
        new ApiResponse<T>
        {
            Payload = payload,
            Pagination = pagination
        };
}
public class Pagination
{
    public int Records { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Records / PageSize) : 0;

    public string NextPage { get; set; } = string.Empty;
    public string PreviousPage { get; set; } = string.Empty;

    // Optional - advanced support
    public string? NextCursor { get; set; }
    public string? PreviousCursor { get; set; }
}