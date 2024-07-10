namespace Application.Common.Models
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public object Entity { get; set; }
        public string Error { get; set; }
        public string ExceptionError { get; set; }
        public string Message { get; set; }

        internal Result(bool succeeded, string message, object entity = default, string exception = null)
        {
            Succeeded = succeeded;
            Message = message;
            ExceptionError = exception;
            Entity = entity;
        }

        internal Result(bool succeeded, object entity = default)
        {
            Succeeded = succeeded;
            Entity = entity;
        }

        public static Result Success(object entity)
        {
            return new Result(true, "Request was executed successfully!", entity);
        }

        public static Result Success(Type request, string message)
        {
            return new Result(true, message);
        }

        public static Result Success(object entity, string message, Type request)
        {
            return new Result(true, message, entity);
        }

        public static Result Success(object entity, Type request)
        {
            return new Result(true, entity);
        }

        public static Result Success<T>(string message)
        {
            return new Result(true, message);
        }

        public static Result Success<T>(string message, object entity)
        {
            return new Result(true, message, entity);
        }

        public static Result Success<T>(object entity, T request)
        {
            return new Result(true, entity);
        }

        public static Result Success<T>(object entity, T request, string message)
        {
            return new Result(true, message, entity);
        }

        public static Result Success<T>(object entity, string message, T request)
        {
            return new Result(true, message, entity);
        }

        public static Result Failure(Type request, string error)
        {
            return new Result(false, error);
        }

        public static Result Failure(Type request, string prefixMessage, Exception ex)
        {
            return new Result(false, $"{prefixMessage} Error: {ex?.Message + Environment.NewLine + ex?.InnerException?.Message}");
        }

        public static Result Failure<T>(string error)
        {
            return new Result(false, error);
        }

        public static Result Failure<T>(string prefixMessage, Exception ex)
        {
            return new Result(false, $"{prefixMessage}");
        }

        public static Result Failure<T>(object entity)
        {
            return new Result(false, entity);
        }
    }
}
