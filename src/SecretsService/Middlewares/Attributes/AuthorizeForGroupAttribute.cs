using System;

using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Middlewares.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AuthorizeForGroupAttribute : TypeFilterAttribute
	{
		public AuthorizeForGroupAttribute(RoleAccessStrategy roleAccessStrategy = RoleAccessStrategy.Any)
			: base(typeof(OrdinaryAccessAuthorizationFilter))
		{
			Arguments = new object[] { roleAccessStrategy };
		}
	}
}