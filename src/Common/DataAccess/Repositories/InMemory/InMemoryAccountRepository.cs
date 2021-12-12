using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;


namespace Keebox.Common.DataAccess.Repositories.InMemory
{
	public class InMemoryAccountRepository : InMemoryRepositoryBase<Account>, IAccountRepository
	{
		public bool Exists(string accountName)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(accountName);

			return Storage.Any(x => x.Name.Equals(accountName));
		}

		public bool Exists(Guid accountId)
		{
			return Storage.Any(x => x.Id == accountId);
		}

		public bool ExistsWithToken(string tokenHash)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(tokenHash);

			return Storage.Any(x => x.TokenHash is not null && x.TokenHash.Equals(tokenHash));
		}

		public Guid Create(Account account)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(account.Name);

			if (account.Id == Guid.Empty)
			{
				account.Id = Guid.NewGuid();
			}

			Storage.Add(account);

			return account.Id;
		}

		public void Delete(Guid accountId)
		{
			var index = Storage.FindIndex(x => x.Id == accountId);

			if (index == -1)
			{
				throw new NotFoundException($"Cannot find account with id {accountId}");
			}

			Storage.RemoveAt(index);
		}

		public Account GetByName(string accountName)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(accountName);

			return Storage.Single(x => x.Name.Equals(accountName));
		}

		public Account GetByTokenHash(string tokenHash)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(tokenHash);

			return Storage.Single(x => x.TokenHash is not null && x.TokenHash.Equals(tokenHash));
		}

		public void Update(Account account)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(account.Name);

			var index = Storage.FindIndex(x => x.Id == account.Id);

			if (index == -1)
			{
				return;
			}

			Storage[index] = account;
		}

		public IEnumerable<Account> List()
		{
			return Storage;
		}

		public Account Get(Guid accountId)
		{
			return Storage.Single(x => x.Id == accountId);
		}
	}
}