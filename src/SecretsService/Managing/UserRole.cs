using System;


namespace Keebox.SecretsService.Managing
{
	public class UserRole
	{
		public Guid RoleId { get; init; }

		public bool IsSystemRole { get; init; }
	}
}