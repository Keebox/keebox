using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;


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
			var hashedToken = _cryptoService.GetHash(token);

			return _accountRepository.ExistsWithToken(hashedToken);
		}

		private readonly ICryptoService _cryptoService;
		private readonly IAccountRepository _accountRepository;
	}
}