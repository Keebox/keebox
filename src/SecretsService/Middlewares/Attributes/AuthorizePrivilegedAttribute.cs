using System;

using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Middlewares.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AuthorizePrivilegedAttribute : TypeFilterAttribute
	{
		public AuthorizePrivilegedAttribute() : base(typeof(PrivilegedAccessAuthorizationFilter)) { }
	}
}