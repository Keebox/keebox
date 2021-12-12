using System;


namespace Keebox.SecretsService.Models.EntityCreation
{
	[Serializable]
	public record RoleCreationPayload
	{
		public string? Name { get; init; }
	}
}