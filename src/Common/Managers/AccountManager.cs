using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;
using Keebox.Common.Security;


namespace Keebox.Common.Managers
{
	public class AccountManager : IAccountManager
	{
		public AccountManager(IRepositoryContext repositoryContext, ICryptoService cryptoService)
		{
			_cryptoService = cryptoService;
			_accountRepository = repositoryContext.GetAccountRepository();
			_assignmentRepository = repositoryContext.GetAssignmentRepository();
			_roleRepository = repositoryContext.GetRoleRepository();
		}

		public IEnumerable<Account> GetAccounts()
		{
			return _accountRepository.List();
		}

		public void CreateTokenAccount(string name, string token)
		{
			_accountRepository.Create(new Account
			{
				Name = name,
				TokenHash = _cryptoService.GetHash(token)
			});
		}

		public void UpdateAccount(Account account)
		{
			EnsureAccountExists(account.Id);

			_accountRepository.Update(account);
		}

		public void DeleteAccount(Guid accountId)
		{
			EnsureAccountExists(accountId);

			_accountRepository.Delete(accountId);
		}

		public Account GetAccount(Guid accountId)
		{
			EnsureAccountExists(accountId);

			return _accountRepository.Get(accountId);
		}

		public void AssignRoleToAccount(Guid roleId, Guid accountId)
		{
			EnsureAccountExists(accountId);
			EnsureRoleExists(roleId);

			if (_assignmentRepository.IsAccountAlreadyAssigned(accountId, roleId)) return;

			_assignmentRepository.Assign(accountId, roleId);
		}

		private void EnsureAccountExists(Guid accountId)
		{
			if (!_accountRepository.Exists(accountId))
			{
				throw new NotFoundException("Account not found.");
			}
		}

		private void EnsureRoleExists(Guid roleId)
		{
			if (!_roleRepository.Exists(roleId))
			{
				throw new NotFoundException("Role not found.");
			}
		}

		private readonly IRoleRepository _roleRepository;
		private readonly IAccountRepository _accountRepository;
		private readonly IAssignmentRepository _assignmentRepository;

		private readonly ICryptoService _cryptoService;
	}
}