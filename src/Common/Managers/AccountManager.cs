using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;
using Keebox.Common.Helpers;


namespace Keebox.Common.Managers
{
	public class AccountManager : IAccountManager
	{
		public AccountManager(IRepositoryContext repositoryContext, ICryptoService cryptoService)
		{
			_cryptoService = cryptoService;
			_accountRepository = repositoryContext.GetAccountRepository();
		}

		public IEnumerable<Account> GetAccounts()
		{
			return _accountRepository.List();
		}

		public void CreateTokenAccount(string token)
		{
			_accountRepository.Create(new Account
			{
				TokenHash = _cryptoService.GetHash(token)
			});
		}

		public void UpdateAccount(Account account)
		{
			EnsureAccount(account.Id);

			_accountRepository.Update(account);
		}

		public void DeleteAccount(Guid accountId)
		{
			EnsureAccount(accountId);

			_accountRepository.Delete(accountId);
		}

		public Account GetAccount(Guid accountId)
		{
			EnsureAccount(accountId);

			return _accountRepository.Get(accountId);
		}

		private void EnsureAccount(Guid accountId)
		{
			if (!_accountRepository.Exists(accountId))
			{
				throw new NotFoundException("Account not found");
			}
		}

		private readonly IAccountRepository _accountRepository;
		private readonly ICryptoService _cryptoService;
	}
}