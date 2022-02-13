namespace Keebox.SecretsService.Specs.Lib.Models;

public record Account
{
	public string Id { get; init; }

	public int Type { get; init; }

	public string Name { get; init; }

	public bool GenerateToken { get; init; }
}