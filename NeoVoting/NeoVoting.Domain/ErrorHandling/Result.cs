using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.ErrorHandling
{
    public class Result<T>
    {
        private Result(bool isSuccess, Error error, T value)
        {
            IsSuccess = isSuccess;
            Error = error;
            Value = value;
        }

        public bool IsSuccess { get; }

        //public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public T Value { get; }

        public static Result<T> Success(T value) => new(true, Error.None, value);

        public static Result<T> Failure(Error error) => new(false, error, default!);
    }
}
