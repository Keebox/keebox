using System;
using System.Linq;
using System.Security.Claims;


namespace Keebox.SecretsService.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static Guid[] GetNonSystemRoles(this ClaimsPrincipal principal)
		{
			return principal.Claims
				.Where(c => c.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) && Guid.TryParse(c.Value, out _))
				.Select(c => Guid.Parse(c.Value))
				.ToArray();
		}

		public static Guid GetUserIdentifier(this ClaimsPrincipal principal)
		{
			return Guid.Parse(principal.Claims
				.Single(c => c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase) && Guid.TryParse(c.Value, out _))
				.Value);
		}
	}
}