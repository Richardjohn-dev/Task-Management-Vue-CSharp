namespace HangfireParallelTasks.Domain.Primitives;

public enum ResultStatus
{
    Ok,
    Error,
    Forbidden,
    Unauthorized,
    Invalid,
    NotFound,
    Unhealthy,
    HttpResponseException,
    VendorResponseFailure
}

public interface IResult
{
    ResultStatus Status { get; }

    IEnumerable<string> Errors { get; }
}
public record EndpointResponse<T> : IResult
{
    public T Value { get; }
    public ResultStatus Status { get; protected set; }
    public string SuccessMessage { get; protected set; } = string.Empty;
    public IEnumerable<string> Errors { get; protected set; } = new List<string>();
    public bool IsSuccess => Status == ResultStatus.Ok;

    protected EndpointResponse(ResultStatus status)
    {
        Status = status;
    }

    protected EndpointResponse()
    {

    }


    public EndpointResponse(T value) => Value = value;

    public static implicit operator EndpointResponse<T>(Result result)
    {
        return new EndpointResponse<T>(default(T))
        {
            Status = result.Status,
            Errors = result.Errors,
            SuccessMessage = result.SuccessMessage,
        };
    }

    public static implicit operator T(EndpointResponse<T> result)
    {
        return result.Value;
    }

    public static implicit operator EndpointResponse<T>(T value)
    {
        return new EndpointResponse<T>(value);
    }

    public static EndpointResponse<T> Success(T value)
    {
        return new EndpointResponse<T>(value);
    }

    public static Result SuccessWithMessage(string message)
    {
        return new Result(ResultStatus.Ok)
        {
            SuccessMessage = message
        };
    }

    public static EndpointResponse<T> Error(params string[] errorMessages)
    {
        return new EndpointResponse<T>(ResultStatus.Error)
        {
            Errors = errorMessages
        };
    }


    public static EndpointResponse<T> NotFound(params string[] errorMessages)
    {
        return new EndpointResponse<T>(ResultStatus.NotFound)
        {
            Errors = errorMessages
        };
    }

}


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

    public static Result HttpError(params string[] errorMessages)
    {
        return new Result(ResultStatus.VendorResponseFailure)
        {
            Errors = errorMessages,
        };
    }

}


