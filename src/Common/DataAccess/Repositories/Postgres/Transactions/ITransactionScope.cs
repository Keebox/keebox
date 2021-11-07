using System;


namespace Keebox.Common.DataAccess.Repositories.Postgres.Transactions
{
	public interface ITransactionScope : IDisposable
	{
		void Commit();
	}
}