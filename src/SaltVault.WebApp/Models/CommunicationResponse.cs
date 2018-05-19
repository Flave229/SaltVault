using System.Collections.Generic;
using SaltVault.Core.Exception;

namespace SaltVault.WebApp.Models
{
    public class CommunicationResponse
    {
        public Error Error { get; set; }
        public bool HasError { get; set; }
        public List<string> Notifications { get; set; }

        public CommunicationResponse()
        {
            Notifications = new List<string>();
        }

        public void AddError(string message)
        {
            Error = new Error { Message = message };
            HasError = true;
        }

        public void AddError(string message, ErrorCode code)
        {
            Error = new Error
            {
                Message = message,
                ErrorCode = code
            };
            HasError = true;
        }

        public void AddError(string message, object originalRequest)
        {
            Error = new Error
            {
                Message = message,
                OriginalRequest = originalRequest
            };
            HasError = true;
        }

        public void AddError(string message, object originalRequest, ErrorCode code)
        {
            Error = new Error
            {
                Message = message,
                OriginalRequest = originalRequest,
                ErrorCode = code
            };
            HasError = true;
        }
    }

    public class Error
    {
        public string Message { get; set; }
        public object OriginalRequest { get; set; }
        public ErrorCode ErrorCode { get; set; }
    }
}