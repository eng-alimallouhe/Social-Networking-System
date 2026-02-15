using SNS.Common.StatusCodes;
using System.Text.Json.Serialization;

namespace SNS.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }

        [JsonIgnore]
        public bool IsFailure => !IsSuccess;

        public StatusCode StatusCode { get; }


        public static Result Success(StatusCode statusCode)
            => new Result(true, statusCode);

        public static Result Failure(StatusCode statusCode)
            => new Result(false, statusCode);

        protected Result(bool isSuccess, StatusCode statusCode)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
        }
    }

    public class Result<TEntity> : Result
    {
        public TEntity? Value { get; }

        public static Result<TEntity> Success(TEntity value, StatusCode statusCode) => new Result<TEntity>(true, value, statusCode);
        public static new Result<TEntity> Failure(StatusCode statusCode) => new Result<TEntity>(false, default!, statusCode);

        private Result(bool isSuccess, TEntity? value, StatusCode statusCode) : base(isSuccess, statusCode)
        {
            Value = value;
        }
    }
}