using System;
using System.Collections.Generic;
using System.Net;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Helpers;
using Keebox.Common.Managers;
using Keebox.Common.Types;
using Keebox.SecretsService.Middlewares.Attributes;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.Models.EntityCreation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSwag.Annotations;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Authenticate, AuthorizePrivileged]
	[Route(RouteMap.Account.Base)]
	public class AccountController : ControllerBase
	{
		public AccountController(IAccountManager accountManager, ITokenService tokenService, ILogger<AccountController> logger)
		{
			_accountManager = accountManager;
			_tokenService = tokenService;
			_logger = logger;
		}

		[HttpGet("{accountId:guid}")]
		[OpenApiOperation("Get account by id")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(Account))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult<Account> GetAccount([FromRoute] Guid accountId)
		{
			_logger.LogInformation($"Getting information about account {accountId}.");

			return Ok(_accountManager.GetAccount(accountId));
		}

		[HttpGet]
		[OpenApiOperation("Get all accounts")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<Account>))]
		public ActionResult<IEnumerable<Account>> ListAccounts()
		{
			_logger.LogInformation("Getting list of all accounts.");

			return Ok(_accountManager.GetAccounts());
		}

		[HttpPost]
		[OpenApiOperation("Create account")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(string))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		public ActionResult<string> CreateAccount([FromBody] AccountCreationPayload creationPayload)
		{
			if (creationPayload.Type == null) throw new ArgumentException("Type is not provided.");
			if (creationPayload.Name == null) throw new ArgumentException("Name is not provided.");

			_logger.LogInformation($"Creating account {creationPayload}.");

			switch (creationPayload.Type)
			{
				case AccountType.Token:
					string token;

					if (creationPayload.GenerateToken.HasValue && creationPayload.GenerateToken.Value)
					{
						token = _tokenService.GenerateStatelessToken();
					}
					else
					{
						token = creationPayload.Token ?? throw new ArgumentException("Token is not provided.");
					}

					var accountId = _accountManager.CreateTokenAccount(creationPayload.Name, token);

					return Created($"account/{accountId}", token);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		[HttpPut("{accountId:guid}")]
		[OpenApiOperation("Update account by id", "Provided account replaces existing account with given id")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult ReplaceAccount([FromBody] Account account, [FromRoute] Guid accountId)
		{
			if (accountId != account.Id) throw new ArgumentException("Ids do not match");

			_logger.LogInformation($"Updating account with id {accountId}.");

			_accountManager.UpdateAccount(account);

			return NoContent();
		}

		[HttpDelete("{accountId:guid}")]
		[OpenApiOperation("Delete account by id")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult DeleteAccount([FromRoute] Guid accountId)
		{
			_logger.LogInformation($"Deleting account with id {accountId}.");

			_accountManager.DeleteAccount(accountId);

			return NoContent();
		}

		[HttpPost(RouteMap.Account.Assign)]
		[OpenApiOperation("Assign role to account")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		public ActionResult AssignRoleToAccount([FromBody] AssignCreationPayload payload)
		{
			if (payload.RoleId == null) throw new ArgumentException("Role id is not provided.");
			if (payload.AccountId == null) throw new ArgumentException("Account id is not provided.");

			_logger.LogInformation($"Assigning {payload.RoleId} role to account {payload.AccountId}.");

			_accountManager.AssignRoleToAccount(payload.RoleId.Value, payload.AccountId.Value);

			return NoContent();
		}

		private readonly IAccountManager _accountManager;
		private readonly ITokenService _tokenService;

		private readonly ILogger<AccountController> _logger;
	}
}