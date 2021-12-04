﻿using System;
using System.Linq;

using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.Exceptions;
using Keebox.Common.Helpers;
using Keebox.Common.Types;
using Keebox.SecretsService.Managing;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace Keebox.SecretsService.RequestFiltering
{
	public class AuthenticationFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			var serviceProvider = context.HttpContext.RequestServices;

			var tokenService = serviceProvider.GetRequiredService<ITokenValidator>();
			var cryptoService = serviceProvider.GetRequiredService<ICryptoService>();

			var repositoryContext = serviceProvider.GetRequiredService<IRepositoryContext>();
			var accountRepository = repositoryContext.GetAccountRepository();
			var assignmentRepository = repositoryContext.GetAssignmentRepository();

			var configuration = serviceProvider.GetRequiredService<Configuration>();

			var authenticationType = ResolveAuthenticationType(context.HttpContext);
			Guid userId;

			switch (authenticationType)
			{
				case AuthenticationType.Header:
				case AuthenticationType.Cookie:
				{
					var token = ResolveTokenFrom(context.HttpContext, authenticationType);

					if (configuration.RootToken!.Equals(token, StringComparison.OrdinalIgnoreCase))
						return;

					var tokenHash = cryptoService.GetHash(token);

					// move to authorization
					if (context.HttpContext.Request.Path.ToString().StartsWith("/account"))
						throw new RestrictedAccessException("Account management only for admins.");

					if (!tokenService.ValidateHash(tokenHash))
						throw new UnauthorizedException("Token is not valid.");

					userId = accountRepository.GetByTokenHash(tokenHash).Id;

					break;
				}
				case AuthenticationType.None:
					throw new UnauthorizedException("Authentication was not successful.");

				case AuthenticationType.Certificate:
				default:
					throw new ArgumentOutOfRangeException();
			}

			var roleIds = assignmentRepository.GetRolesByAccount(userId).ToArray();
			context.HttpContext.User = new UserPrincipal(userId, roleIds);
		}

		public void OnActionExecuted(ActionExecutedContext context) { }

		private static AuthenticationType ResolveAuthenticationType(HttpContext context)
		{
			if (context.Connection.ClientCertificate != null)
				return AuthenticationType.Certificate;

			if (context.Request.Headers.ContainsKey(TokenHeader))
				return AuthenticationType.Header;

			if (context.Request.Cookies.ContainsKey(TokenHeader))
			{
				return AuthenticationType.Cookie;
			}

			return AuthenticationType.None;
		}

		private static string ResolveTokenFrom(HttpContext context, AuthenticationType type)
		{
			return type switch
			{
				AuthenticationType.Header => context.Request.Headers[TokenHeader],
				AuthenticationType.Cookie => context.Request.Cookies[TokenHeader]!,
				_                         => throw new ArgumentOutOfRangeException()
			};
		}

		private const string TokenHeader = "X-Token";
	}
}