using System;


namespace Keebox.SecretsService.Models
{
	[Serializable]
	public record LoginPayload
	{
		public string? Token { get; init; }
	};
}