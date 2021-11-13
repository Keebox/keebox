using System;

using Keebox.Common.Exceptions;
using Keebox.Common.Helpers;
using Keebox.Common.Types;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace Keebox.SecretsService.RequestFiltering
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AuthenticateAttribute : Attribute, IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			var serviceProvider = context.HttpContext.RequestServices;

			var tokenService = serviceProvider.GetRequiredService<ITokenValidator>();
			var configuration = serviceProvider.GetRequiredService<Configuration>();

			var authenticationType = ResolveAuthenticationType(context.HttpContext);

			switch (authenticationType)
			{
				case AuthenticationType.Token:
				{
					var token = context.HttpContext.Request.Headers[TokenHeader];

					if (configuration.RootToken!.Equals(token, StringComparison.OrdinalIgnoreCase))
						return;

					if (context.HttpContext.Request.Path.ToString().StartsWith("/account"))
						throw new RestrictedAccessException("Account management only for admins.");

					if (!tokenService.Validate(token))
						throw new UnauthorizedException("Token is not valid.");

					break;
				}
				case AuthenticationType.Cookie:
				{
					var token = context.HttpContext.Request.Cookies[TokenHeader];

					if (configuration.RootToken!.Equals(token, StringComparison.OrdinalIgnoreCase))
						return;

					if (context.HttpContext.Request.Path.ToString().StartsWith("/account"))
						throw new RestrictedAccessException("Account management only for admins.");

					if (!tokenService.Validate(token))
						throw new UnauthorizedException("Token is not valid.");

					break;
				}
				case AuthenticationType.None:
					throw new UnauthorizedException("Authentication was not successful.");

				case AuthenticationType.Certificate:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void OnActionExecuted(ActionExecutedContext context) { }

		private static AuthenticationType ResolveAuthenticationType(HttpContext context)
		{
			if (context.Connection.ClientCertificate != null)
				return AuthenticationType.Certificate;

			if (context.Request.Headers.ContainsKey(TokenHeader))
				return AuthenticationType.Token;

			if (context.Request.Cookies.ContainsKey(TokenHeader))
			{
				return AuthenticationType.Cookie;
			}

			return AuthenticationType.None;
		}

		private const string TokenHeader = "X-Token";
	}
}