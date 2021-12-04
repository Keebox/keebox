using System;
using System.Security.Claims;
using System.Security.Principal;


namespace Keebox.SecretsService.Managing
{
	public class UserPrincipal : ClaimsPrincipal
	{
		public UserPrincipal(Guid userId, Guid[] roleIds)
		{
			Name = userId.ToString();
			RoleIds = roleIds;
		}

		public string Name { get; }

		public Guid[] RoleIds { get; }
	}
}