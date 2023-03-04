namespace CardsAgainstMySanity.SharedKernel;

using System;

public interface IResult
{
    bool Succeeded { get; }

    bool Failed { get; }
}

public abstract class Result : IResult
{
    protected Result(bool succeeded)
        => Succeeded = succeeded;

    public bool Succeeded { get; private set; }

    public bool Failed => !Succeeded;
}

public abstract class Result<TData> : Result, IResult
{
    protected Result(TData data) : base(true)
        => Data = data;

    protected Result() : base(false)
    {
    }

    private TData _data;

    public TData Data
    {
        get => Succeeded ? _data : throw new InvalidOperationException($"You can't access .{nameof(Data)} when .{nameof(Succeeded)} is false");
        private set => _data = value;
    }
}

public abstract class Result<TData, TError> : Result, IResult
{
    protected Result(TData data) : base(true)
        => Data = data;

    protected Result(TError error) : base(false)
        => Error = error;

    private TData _data;
    private TError _error;

    public TData Data
    {
        get => Succeeded ? _data : throw new InvalidOperationException($"You can't access .{nameof(Data)} when .{nameof(Succeeded)} is false");
        private set => _data = value;
    }

    public TError Error
    {
        get => Failed ? _error : throw new InvalidOperationException($"You can't access .{nameof(Error)} when .{nameof(Failed)} is false");
        private set => _error = value;
    }
}
