using System;


namespace Keebox.SecretsService.Models
{
	[Serializable]
	public record Config
	{
		public bool EnableWebUi { get; init; }

		public string DefaultFormat { get; init; } = default!;

		public string Engine { get; init; } = default!;
	}
}