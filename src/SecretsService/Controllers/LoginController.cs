using System;
using System.Linq;

using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Helpers;
using Keebox.Common.Security;
using Keebox.SecretsService.Models.EntityCreation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Route(RouteMap.Login)]
	public class LoginController : ControllerBase
	{
		public LoginController(IRepositoryContext repositoryContext, ICryptoService cryptoService, ITokenService tokenService)
		{
			_tokenService = tokenService;
			_cryptoService = cryptoService;
			_accountRepository = repositoryContext.GetAccountRepository();
			_assignmentRepository = repositoryContext.GetAssignmentRepository();
			_roleRepository = repositoryContext.GetRoleRepository();
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<string> Login([FromBody] LoginPayload loginPayload)
		{
			if (loginPayload.Token == null) throw new ArgumentException("Account token is not provided");

			var tokenHash = _cryptoService.GetHash(loginPayload.Token);
			var account = _accountRepository.GetByTokenHash(tokenHash);

			var assignments = _assignmentRepository.GetRolesByAccount(account.Id);
			var roles = _roleRepository.List().Where(r => assignments.Contains(r.Id));

			var jwtToken = _tokenService.GenerateJwtToken(account.Id, roles.ToArray());

			return Ok(jwtToken);
		}

		private readonly IAccountRepository _accountRepository;
		private readonly IAssignmentRepository _assignmentRepository;
		private readonly IRoleRepository _roleRepository;
		private readonly ICryptoService _cryptoService;
		private readonly ITokenService _tokenService;
	}
}