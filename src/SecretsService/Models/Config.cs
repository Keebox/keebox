namespace Keebox.SecretsService.Models
{
	public record Config
	{
		public bool EnableWebUi { init; get; }

		public string DefaultFormat { init; get; }

		public string Engine { init; get; }
	}
}