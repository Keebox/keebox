using System;
using System.Linq;
using System.Net;

using Keebox.Common;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;
using Keebox.Common.Helpers;
using Keebox.Common.Security;
using Keebox.SecretsService.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSwag.Annotations;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Route(RouteMap.Login)]
	public class LoginController : ControllerBase
	{
		public LoginController(
			IRepositoryContext repositoryContext, ICryptoService cryptoService, ITokenService tokenService, ITokenValidator tokenValidator,
			ILogger<LoginController> logger)
		{
			_tokenService = tokenService;
			_tokenValidator = tokenValidator;
			_logger = logger;
			_cryptoService = cryptoService;

			_roleRepository = repositoryContext.GetRoleRepository();
			_accountRepository = repositoryContext.GetAccountRepository();
			_assignmentRepository = repositoryContext.GetAssignmentRepository();
		}

		[HttpPost]
		[SwaggerResponse(HttpStatusCode.OK, typeof(string), IsNullable = false)]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), IsNullable = false)]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error), IsNullable = false)]
		[OpenApiOperation("Login", "Log in to Keebox and receive access token")]
		public ActionResult<string> Login([FromBody] LoginPayload loginPayload)
		{
			_logger.LogInformation("Login attempt.");

			if (loginPayload.Token == null) throw new ArgumentException("Account token is not provided");

			var tokenHash = _cryptoService.GetHash(loginPayload.Token);

			if (!_tokenValidator.ValidateHash(tokenHash)) throw new NotFoundException("Account with this token does not exists.");

			var account = _accountRepository.GetByTokenHash(tokenHash);

			var assignments = _assignmentRepository.GetRolesByAccount(account.Id);
			var roles = _roleRepository.List().Where(r => assignments.Contains(r.Id));

			var jwtToken = _tokenService.GenerateJwtToken(account.Id, roles.ToArray(), TimeSpan.FromDays(1));

			Response.Cookies.Append(Constants.JwtCookieName, jwtToken);

			return Ok(jwtToken);
		}

		private readonly ICryptoService _cryptoService;

		private readonly IRoleRepository _roleRepository;
		private readonly IAccountRepository _accountRepository;
		private readonly IAssignmentRepository _assignmentRepository;

		private readonly ITokenValidator _tokenValidator;
		private readonly ITokenService _tokenService;

		private readonly ILogger<LoginController> _logger;
	}
}