using System;


namespace Keebox.SecretsService.Models.EntityCreation
{
	[Serializable]
	public record LoginPayload
	{
		public string? Token { get; init; }
	};
}