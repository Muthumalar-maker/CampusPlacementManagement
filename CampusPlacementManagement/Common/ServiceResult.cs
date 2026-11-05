
namespace CampusPlacementManagement.Common
{
    public class ServiceResult<T>
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public static ServiceResult<T> Success(
            T data,
            string message,
            int statusCode = 200)
        {
            return new ServiceResult<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(
            string message,
            int statusCode)
        {
            return new ServiceResult<T>
            {
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
