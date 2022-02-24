namespace CardsAgainstMySanity.SharedKernel;

public interface IResult
{
    bool Succeeded { get; }
    bool Failed { get; }
}

public class Result : IResult
{
    public bool Succeeded { get; }
    public bool Failed => !this.Succeeded;

    protected Result(bool succeeded) => this.Succeeded = succeeded;

    public static Result Ok() => new(true);

    public static Result Fail() => new(false);
}

public class Result<TData> : Result
{
    public TData Data { get; }

    protected Result(bool succeeded, TData data) : base(succeeded) => this.Data = data;

    public static Result<TData> Ok(TData data) => new(true, data);

    public static new Result<TData> Fail() => new(false, default);
}

public class Result<TData, TError> : Result<TData>
{
    public TError Error { get; }

    protected Result(bool succeeded, TData data, TError error) : base(succeeded, data) => this.Error = error;

    public static new Result<TData, TError> Ok(TData data) => new(true, data, default);

    public static new Result<TData, TError> Fail() => new(false, default, default);

    public static Result<TData, TError> Fail(TError error) => new(false, default, error);
}
