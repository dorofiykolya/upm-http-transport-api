using System;
using System.Net.Http;

namespace HttpTransport.Rpc
{
    public interface IRemoteRequest
    {
        void Request(
                AsyncResponse response,
                Type requestType,
                object request,
                Type responseType,
                HttpMethod method,
                string path
        );
    }
}