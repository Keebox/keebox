using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Helpers;
using Keebox.Common.Managers;
using Keebox.Common.Types;
using Keebox.SecretsService.Models.EntityCreation;
using Keebox.SecretsService.RequestFiltering;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Route(RouteMap.Account)]
	[Authenticate] [AuthorizePrivileged]
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
		public ActionResult<string> CreateAccount([FromBody] AccountCreationPayload creationPayload)
		{
			if (creationPayload.Type == null) throw new ArgumentException("Type is not provided.");

			switch (creationPayload.Type)
			{
				case AccountType.Token:
					string token;

					if (creationPayload.Generate != null && (bool)creationPayload.Generate)
					{
						token = _tokenService.GenerateToken();
					}
					else
					{
						token = creationPayload.Token ?? throw new ArgumentException("Token is not provided");
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
			if (accountId != account.Id) throw new ArgumentException("Ids do not match");

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

		[HttpPost("assign")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult AssignRoleToAccount([FromBody] AssignCreationPayload payload)
		{
			if (payload.RoleId == null) throw new ArgumentException("Role id is not provided.");
			if (payload.AccountId == null) throw new ArgumentException("Account id is not provided.");

			_accountManager.AssignRoleToAccount((Guid)payload.RoleId, (Guid)payload.AccountId);

			return Ok();
		}

		private readonly IAccountManager _accountManager;
		private readonly ITokenService _tokenService;
	}
}