using System;

namespace Keebox.SecretsService.Models
{
    public record Error
    {
        public Error(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
            Timestamp = DateTime.Now;
        }

        public int StatusCode { init; get; }

        public string Message { init; get; }

        public DateTime Timestamp { init; get; }
    }
}