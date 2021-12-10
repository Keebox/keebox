using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Security;


namespace Keebox.Common.Helpers
{
	public class TokenValidator : ITokenValidator
	{
		public TokenValidator(ICryptoService cryptoService, IRepositoryContext repositoryContext)
		{
			_cryptoService = cryptoService;
			_accountRepository = repositoryContext.GetAccountRepository();
		}

		public bool Validate(string token)
		{
			var tokenHash = _cryptoService.GetHash(token);

			return _accountRepository.ExistsWithToken(tokenHash);
		}

		public bool ValidateHash(string tokenHash)
		{
			return _accountRepository.ExistsWithToken(tokenHash);
		}

		private readonly ICryptoService _cryptoService;
		private readonly IAccountRepository _accountRepository;
	}
}