namespace Keebox.SecretsService.Specs.Lib.Models;

public record Account
{
	public int Type { get; init; }

	public string Name { get; init; }

	public bool GenerateToken { get; init; }
}