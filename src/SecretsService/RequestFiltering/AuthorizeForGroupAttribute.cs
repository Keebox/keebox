using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.RequestFiltering
{
	public class AuthorizeForGroupAttribute : TypeFilterAttribute
	{
		public AuthorizeForGroupAttribute(RoleAccessStrategy roleAccessStrategy = RoleAccessStrategy.Any)
			: base(typeof(AuthorizationFilter))
		{
			Arguments = new object[] { roleAccessStrategy };
		}
	}
}