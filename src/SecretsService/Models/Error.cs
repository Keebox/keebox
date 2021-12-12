using System;


namespace Keebox.SecretsService.Models
{
	[Serializable]
	public record Error
	{
		public Error(int statusCode, string message)
		{
			StatusCode = statusCode;
			Message = message;
			Timestamp = DateTime.Now;
		}

		public int StatusCode { get; init; }

		public string Message { get; init; }

		public DateTime Timestamp { get; init; }
	}
}