using System;


namespace Keebox.SecretsService.Models.EntityCreation
{
	[Serializable]
	public record AssignCreationPayload
	{
		public Guid? AccountId { get; init; }

		public Guid? RoleId { get; init; }
	}
}