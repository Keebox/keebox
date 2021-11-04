using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Helpers;


namespace Keebox.Common.Managers
{
	public class AccountManager : IAccountManager
	{
		private readonly IAccountRepository _accountRepository;
		private readonly ICryptoService _cryptoService;

		public AccountManager(IRepositoryContext repositoryContext, ICryptoService cryptoService)
		{
			_cryptoService = cryptoService;
			_accountRepository = repositoryContext.GetAccountRepository();
		}

		public void CreateTokenAccount(string token)
		{
			_accountRepository.Create(new Account
			{
				TokenHash = _cryptoService.GetHash(token)
			});
		}

		public void DeleteAccount(Guid accountId)
		{
			_accountRepository.Delete(accountId);
		}

		public IEnumerable<Account> GetAccounts()
		{
			return _accountRepository.List();
		}
	}
}