using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.services.utils
{
    // Базовый класс Result
    public class Result
    {
        protected Result()
        {
            IsSuccess = true;
        }

        protected Result(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }
        public string ErrorMessage { get; } = string.Empty;
        public bool IsFailure => !IsSuccess;

        public static Result Success() => new Result();
        public static Result Failure(string errorMessage) => new Result(errorMessage);
    }

    // Generic версия для возврата значения (без nullable)
    public class Result<T> : Result
    {
        private T _value;

        private Result(T value) : base()
        {
            _value = value;
        }

        private Result(string errorMessage) : base(errorMessage)
        {
            _value = default(T);
        }

        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException($"Нельзя получить значение при ошибке: {ErrorMessage}");
                return _value;
            }
        }

        public static Result<T> Success(T value) => new Result<T>(value);
        public static new Result<T> Failure(string errorMessage) => new Result<T>(errorMessage);
    }
}
