using System;


namespace Keebox.SecretsService.Specs.Lib.Models;

[Serializable]
public record Error
{
	public int StatusCode { get; init; }

	public string Message { get; init; }

	public DateTimeOffset Timestamp { get; init; }
}