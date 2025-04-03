
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
public record Result<T> : IResult
{
    public T Value { get; }
    public ResultStatus Status { get; protected set; }
    public string SuccessMessage { get; protected set; } = string.Empty;
    public IEnumerable<string> Errors { get; protected set; } = new List<string>();
    public bool IsSuccess => Status == ResultStatus.Ok;

    protected Result(ResultStatus status)
    {
        Status = status;
    }

    protected Result()
    {

    }


    public Result(T value) => Value = value;

    public static implicit operator Result<T>(Result result)
    {
        return new Result<T>(default(T))
        {
            Status = result.Status,
            Errors = result.Errors,
            SuccessMessage = result.SuccessMessage,
        };
    }

    public static implicit operator T(Result<T> result)
    {
        return result.Value;
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result SuccessWithMessage(string message)
    {
        return new Result(ResultStatus.Ok)
        {
            SuccessMessage = message
        };
    }

    public static Result<T> Error(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.Error)
        {
            Errors = errorMessages
        };
    }


    public static Result<T> NotFound(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.NotFound)
        {
            Errors = errorMessages
        };
    }

}


public record Result : Result<Result>
{
    public Result()
    {

    }




    public static Result Success()
    {
        return new Result();
    }
    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
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