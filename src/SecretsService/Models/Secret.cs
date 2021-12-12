using System;


namespace Keebox.SecretsService.Models
{
	[Serializable]
	public record Secret
	{
		public string Name { get; init; } = default!;

		public string Value { get; init; } = default!;
	}
}