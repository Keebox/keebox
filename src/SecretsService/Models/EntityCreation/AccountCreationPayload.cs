using System;

using Keebox.Common.Types;


namespace Keebox.SecretsService.Models.EntityCreation
{
	[Serializable]
	public record AccountCreationPayload
	{
		public AccountType? Type { get; init; }

		public bool? Generate { get; init; }

		public string? Token { get; init; }
	}
}