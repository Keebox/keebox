using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.Managers
{
	public interface IAccountManager
	{
		Guid CreateTokenAccount(string name, string token);

		void DeleteAccount(Guid accountId);

		IEnumerable<Account> GetAccounts();

		Account GetAccount(Guid accountId);

		void UpdateAccount(Account account);

		void AssignRoleToAccount(Guid roleId, Guid accountId);
	}
}