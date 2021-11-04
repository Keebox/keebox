namespace Keebox.SecretsService.Models
{
	public record Secret
	{
		public string Name { get; init; } = default!;

		public string Value { get; init; } = default!;
	}
}