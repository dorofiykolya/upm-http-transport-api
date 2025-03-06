using System.Net;

namespace HttpTransport.Rpc
{
    public abstract class RequestResult
    {
        public HttpStatusCode HttpStatusCode => (HttpStatusCode)StatusCode;
        public long StatusCode { get; set; }
        public long ErrorCode { get; set; }
        public bool IsNetworkError { get; set; }
        public bool IsOk => StatusCode == 200;
    }

    public class RequestResult<T> : RequestResult
    {
        public T Result { get; set; }

        public RequestResult(long statusCode, long errorCode, object value, bool networkError)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Result = (T)value;
            IsNetworkError = networkError;
        }
    }
}
