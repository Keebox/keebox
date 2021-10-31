using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.Managers
{
	public interface IAccountManager
	{
		void CreateTokenAccount(string token);

		void DeleteAccount(Guid accountId);

		IEnumerable<Account> GetAccounts();
	}
}