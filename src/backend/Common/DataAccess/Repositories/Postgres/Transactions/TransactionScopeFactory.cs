namespace Keebox.Common.DataAccess.Repositories.Postgres.Transactions
{
	public class TransactionScopeFactory : ITransactionScopeFactory
	{
		public ITransactionScope BeginTransaction()
		{
			return new SqlTransactionScope();
		}
	}
}