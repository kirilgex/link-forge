namespace LinkForge.Domain.Shared;

public abstract class AbstractResult
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    protected AbstractResult(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
}

public class Result : AbstractResult
{
    private Result(bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
    
    public TOut Match<TOut>(Func<TOut> onSuccess, Func<Error, TOut> onFailure)
        => IsSuccess ? onSuccess() : onFailure(Error!);
}

public class Result<T> : AbstractResult
{
    public T? Value { get; }

    private Result(bool isSuccess, Error? error, T? value)
        : base(isSuccess, error)
    {
        Value = value;
    }
    
    public Result<TOut> Map<TOut>(Func<T, TOut> transform)
        => IsSuccess ? Result<TOut>.Success(transform(Value!)) : Result<TOut>.Failure(Error!);

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
        => IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    
    public static Result<T> Success(T value) => new(true, null, value);
    public static Result<T> Failure(Error error) => new(false, error, default);
}
