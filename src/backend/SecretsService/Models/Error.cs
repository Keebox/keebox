namespace Keebox.SecretsService.Models
{
	public record Error
	{
		public Error(int statusCode, string message)
		{
			StatusCode = statusCode;
			Message = message;
		}

		public int StatusCode { init; get; }

		public string Message { init; get; }
	}
}