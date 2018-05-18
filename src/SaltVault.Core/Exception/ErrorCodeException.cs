namespace SaltVault.Core.Exception
{
    public class ErrorCodeException : System.Exception
    {
        public override string Message { get; }
        public ErrorCode Code { get; set; }

        public ErrorCodeException(string message, ErrorCode code)
        {
            Message = message;
            Code = code;
        }
    }
}