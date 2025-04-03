namespace HangfireParallelTasks.Domain.Primitives;

public class ApiResponse<T>
{
    public string Status { get; set; }
    public T Value { get; set; }
    public Pagination? Pagination { get; private set; }
    public bool HasPagination => Pagination != null;
    public static ApiResponse<T> Success(T payload)
    {
        return new ApiResponse<T>()
        {
            Value = payload,
            Status = "success",
        };
    }
    public static ApiResponse<T> PagedResponse(T payload, Pagination pagination)
    {
        return new ApiResponse<T>()
        {
            Value = payload,
            Status = "success",
            Pagination = pagination
        };
    }
}

public class Pagination
{

    public int Records { get; set; } = 0;

    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }

    public string NextPage { get; set; } = string.Empty;

    public string PreviousPage { get; set; } = string.Empty;

}
