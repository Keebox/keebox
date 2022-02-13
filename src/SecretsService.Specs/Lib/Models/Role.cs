using System;


namespace Keebox.SecretsService.Specs.Lib.Models;

[Serializable]
public record Role
{
	public Guid Id { get; init; }

	public string Name { get; init; }

	public bool IsSystem { get; init; }
}