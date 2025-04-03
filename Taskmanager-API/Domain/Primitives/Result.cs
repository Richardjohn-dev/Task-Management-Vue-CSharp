namespace HangfireParallelTasks.Domain.Primitives;

public record Result : EndpointResponse<Result>
{
    public Result()
    {

    }




    public static Result Success()
    {
        return new Result();
    }
    public static EndpointResponse<T> Success<T>(T value)
    {
        return new EndpointResponse<T>(value);
    }
    protected internal Result(ResultStatus status)
       : base(status)
    {
    }
    public new static Result Error(params string[] errorMessages)
    {
        return new Result(ResultStatus.Error)
        {
            Errors = errorMessages
        };
    }

    public static Result NotFound() => new(ResultStatus.NotFound)
    {

    };

}



public enum ResultStatus
{
    Ok,
    Error,
    Forbidden,
    Unauthorized,
    Invalid,
    NotFound,
}

public interface IResult
{
    ResultStatus Status { get; }

    IEnumerable<string> Errors { get; }
}