using System.Linq;
using System.Security.Claims;


namespace Keebox.SecretsService.Managing
{
	public class UserPrincipal : ClaimsPrincipal
	{
		public UserPrincipal(UserRole[] roles, bool isRootUser)
		{
			Roles = roles;
			IsRootUser = isRootUser;
		}

		public UserRole[] Roles { get; }

		public bool IsRootUser { get; }

		public bool HasSystemRole()
		{
			return Roles.Any(r => r.IsSystemRole);
		}
	}
}