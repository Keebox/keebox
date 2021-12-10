using System;

using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Middlewares.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AuthenticateAttribute : TypeFilterAttribute
	{
		public AuthenticateAttribute() : base(typeof(AuthenticationFilter)) { }
	}
}