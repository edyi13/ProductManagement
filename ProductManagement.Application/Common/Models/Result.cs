using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Application.Common.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T Data { get; private set; }
        public string ErrorMessage { get; private set; }
        public List<string> Errors { get; private set; }

        private Result(bool isSuccess, T data, string errorMessage, List<string> errors)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            Errors = errors ?? new List<string>();
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, null, null);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error, new List<string> { error });
        }

        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>(false, default, string.Join(", ", errors), errors);
        }
    }

    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        public List<string> Errors { get; private set; }

        private Result(bool isSuccess, string errorMessage, List<string> errors)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Errors = errors ?? new List<string>();
        }

        public static Result Success()
        {
            return new Result(true, null, null);
        }

        public static Result Failure(string error)
        {
            return new Result(false, error, new List<string> { error });
        }

        public static Result Failure(List<string> errors)
        {
            return new Result(false, string.Join(", ", errors), errors);
        }
    }
}
