namespace CardsAgainstMySanity.SharedKernel
{
    public interface IResult
    {
        bool Succeeded { get; }
        bool Failed { get; }
    }

    public class Result : IResult
    {
        public bool Succeeded { get; }
        public bool Failed => !Succeeded;

        protected Result(bool succeeded)
        {
            Succeeded = succeeded;
        }

        public static Result Ok()
        {
            return new Result(true);
        }

        public static Result Fail()
        {
            return new Result(false);
        }
    }

    public class Result<TData> : Result
    {
        public TData Data { get; }

        protected Result(bool succeeded, TData data) : base(succeeded)
        {
            Data = data;
        }

        public static Result<TData> Ok(TData data)
        {
            return new Result<TData>(true, data);
        }

        public static new Result<TData> Fail()
        {
            return new Result<TData>(false, default(TData));
        }
    }

    public class Result<TData, TError> : Result<TData>
    {
        public TError Error { get; }

        protected Result(bool succeeded, TData data, TError error) : base(succeeded, data)
        {
            Error = error;
        }

        public static new Result<TData, TError> Ok(TData data)
        {
            return new Result<TData, TError>(true, data, default(TError));
        }

        public static new Result<TData, TError> Fail()
        {
            return new Result<TData, TError>(false, default, default(TError));
        }

        public static Result<TData, TError> Fail(TError error)
        {
            return new Result<TData, TError>(false, default, error);
        }
    }
}