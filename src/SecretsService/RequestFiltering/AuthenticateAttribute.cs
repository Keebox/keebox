using System;

using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.RequestFiltering
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AuthenticateAttribute : TypeFilterAttribute
	{
		public AuthenticateAttribute() : base(typeof(AuthenticationFilter)) { }
	}
}