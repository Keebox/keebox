using System;


namespace Keebox.SecretsService.Models.EntityCreation
{
	[Serializable]
	public record PermissionCreationPayload
	{
		public Guid? RoleId { get; init; }

		public Guid? GroupId { get; init; }

		public bool? IsReadonly { get; init; }
	}
}