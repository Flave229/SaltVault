using System.Collections.Generic;

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

        public void AddError(string message, object originalRequest)
        {
            Error = new Error
            {
                Message = message,
                OriginalRequest = originalRequest
            };
            HasError = true;
        }
    }

    public class Error
    {
        public string Message { get; set; }
        public object OriginalRequest { get; set; }
    }
}