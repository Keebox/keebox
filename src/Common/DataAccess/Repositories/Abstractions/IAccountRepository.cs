using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.Abstractions
{
	public interface IAccountRepository
	{
		bool Exists(string accountName);

		bool Exists(Guid accountId);

		bool ExistsWithToken(string tokenHash);

		Guid Create(Account account);

		void Delete(Guid accountId);

		Account GetByName(string accountName);

		void Update(Account account);

		IEnumerable<Account> List();

		Account Get(Guid accountId);
	}
}