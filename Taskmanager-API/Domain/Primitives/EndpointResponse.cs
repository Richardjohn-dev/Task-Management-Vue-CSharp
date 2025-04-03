namespace HangfireParallelTasks.Domain.Primitives;

//public record EndpointResponse<T> : IResult
//{
//    public T Payload { get; }
//    public ResultStatus Status { get; protected set; }
//    public string SuccessMessage { get; protected set; } = string.Empty;
//    public IEnumerable<string> Errors { get; protected set; } = new List<string>();
//    public bool Success => Status == ResultStatus.Ok;

//    protected EndpointResponse(ResultStatus status)
//    {
//        Status = status;
//    }

//    protected EndpointResponse()
//    {

//    }


//    public EndpointResponse(T value) => Payload = value;

//    public static implicit operator EndpointResponse<T>(Result result)
//    {
//        return new EndpointResponse<T>(default(T))
//        {
//            Status = result.Status,
//            Errors = result.Errors,
//            SuccessMessage = result.SuccessMessage,
//        };
//    }

//    public static implicit operator T(EndpointResponse<T> result)
//    {
//        return result.Payload;
//    }

//    public static implicit operator EndpointResponse<T>(T value)
//    {
//        return new EndpointResponse<T>(value);
//    }

//    public static EndpointResponse<T> Ok(T value)
//    {
//        return new EndpointResponse<T>(value);
//    }

//    //public static Result Ok(string message)
//    //{
//    //    return new Result(ResultStatus.Ok)
//    //    {
//    //        SuccessMessage = message
//    //    };
//    //}

//    public static EndpointResponse<T> Error(params string[] errorMessages)
//    {
//        return new EndpointResponse<T>(ResultStatus.Error)
//        {
//            Errors = errorMessages
//        };
//    }


//    public static EndpointResponse<T> NotFound(params string[] errorMessages)
//    {
//        return new EndpointResponse<T>(ResultStatus.NotFound)
//        {
//            Errors = errorMessages
//        };
//    }

//}


