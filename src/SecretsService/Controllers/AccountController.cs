using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Helpers;
using Keebox.Common.Managers;
using Keebox.Common.Types;
using Keebox.SecretsService.Exceptions;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.RequestFiltering;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Controllers
{
	[Authenticate]
	[ApiController]
	[Route(RouteMap.Account)]
	public class AccountController : ControllerBase
	{
		public AccountController(IAccountManager accountManager, ITokenService tokenService)
		{
			_accountManager = accountManager;
			_tokenService = tokenService;
		}

		[HttpGet("{accountId:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<Account> GetAccount([FromRoute] Guid accountId)
		{
			return Ok(_accountManager.GetAccount(accountId));
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<Account>> ListAccounts()
		{
			return Ok(_accountManager.GetAccounts());
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<string> CreateAccount([FromRoute] RequestPayload payload)
		{
			var accountType = GetAccountType(payload.Data);

			switch (accountType)
			{
				case AccountType.Token:
					string token;

					if (payload.Data!.ContainsKey("generate") && payload.Data!["generate"] == "true")
					{
						token = _tokenService.GenerateToken();
					}
					else
					{
						if (!payload.Data.ContainsKey("token"))
						{
							throw new ArgumentException("Token is not provided");
						}

						token = (string)payload.Data["token"];
					}

					_accountManager.CreateTokenAccount(token);

					return Ok(token);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		[HttpPut("{accountId:guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult ReplaceAccount([FromBody] Account account, [FromRoute] Guid accountId)
		{
			if (accountId != account.Id)
			{
				throw new ArgumentException("Ids do not match");
			}

			_accountManager.UpdateAccount(account);

			return NoContent();
		}

		[HttpDelete("{accountId:guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteAccount([FromRoute] Guid accountId)
		{
			_accountManager.DeleteAccount(accountId);

			return NoContent();
		}

		private static AccountType GetAccountType(IDictionary<string, object>? body)
		{
			if (body is null || !body.ContainsKey("type"))
			{
				throw new ArgumentException("Type is not provided.");
			}

			return ((string)body["type"]).ToLower() switch
			{
				"token" => AccountType.Token,
				_       => throw new UnsupportedTypeException($"{body["type"]} type is not supported.")
			};
		}

		private readonly IAccountManager _accountManager;
		private readonly ITokenService _tokenService;
	}
}