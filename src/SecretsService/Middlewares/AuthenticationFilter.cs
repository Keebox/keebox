using System;
using System.Linq;
using System.Security.Claims;

using Keebox.Common;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.Exceptions;
using Keebox.Common.Helpers;
using Keebox.Common.Types;
using Keebox.SecretsService.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace Keebox.SecretsService.Middlewares
{
	public class AuthenticationFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			var serviceProvider = context.HttpContext.RequestServices;

			var tokenValidator = serviceProvider.GetRequiredService<ITokenValidator>();
			var accountRepository = serviceProvider.GetRequiredService<IRepositoryContext>().GetAccountRepository();

			var authenticationType = ResolveAuthenticationType(context.HttpContext);

			ClaimsPrincipal? userPrincipal;

			switch (authenticationType)
			{
				case AuthenticationType.Cookie:
				case AuthenticationType.Bearer:
				{
					var token = ResolveTokenFrom(context.HttpContext, authenticationType);
					var isValid = tokenValidator.ValidateJwtToken(token, out userPrincipal);

					if (!isValid) throw new UnauthorizedException("Token is not valid.");

					if (userPrincipal!.IsInRole(FormattedSystemRole.Admin))
					{
						context.HttpContext.User = userPrincipal;
						return;
					}

					var isAccountExists = accountRepository.Exists(userPrincipal.GetUserIdentifier());

					if (!isAccountExists) throw new UnauthorizedException("Account does not exists.");

					break;
				}
				case AuthenticationType.None:
					throw new UnauthorizedException("Authentication was not successful.");

				default:
					throw new ArgumentOutOfRangeException();
			}

			context.HttpContext.User = userPrincipal;
		}

		public void OnActionExecuted(ActionExecutedContext context) { }

		private static AuthenticationType ResolveAuthenticationType(HttpContext context)
		{
			if (context.Connection.ClientCertificate != null)
				return AuthenticationType.Certificate;

			if (context.Request.Cookies.ContainsKey(Constants.JwtCookieName))
				return AuthenticationType.Cookie;

			if (context.Request.Headers.ContainsKey(AuthorizationHeader)
				&& context.Request.Headers[AuthorizationHeader].ToString().StartsWith(BearerToken))
				return AuthenticationType.Bearer;

			return AuthenticationType.None;
		}

		private static string ResolveTokenFrom(HttpContext context, AuthenticationType type)
		{
			return type switch
			{
				AuthenticationType.Cookie => context.Request.Cookies[Constants.JwtCookieName]!,
				AuthenticationType.Bearer => context.Request.Headers[AuthorizationHeader].ToString().Split(BearerToken).Last(),
				_                         => throw new ArgumentOutOfRangeException()
			};
		}

		private const string BearerToken = "Bearer ";
		private const string AuthorizationHeader = "Authorization";
	}
}