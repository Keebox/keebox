using System.Transactions;


namespace Keebox.Common.DataAccess.Repositories.Postgres.Transactions
{
	public sealed class SqlTransactionScope : ITransactionScope
	{
		private readonly TransactionScope _transactionScope;

		public SqlTransactionScope() =>
			_transactionScope = new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions
				{
					Timeout = TransactionManager.DefaultTimeout,
					IsolationLevel = IsolationLevel.ReadCommitted
				},
				TransactionScopeAsyncFlowOption.Enabled);

		public void Commit()
		{
			_transactionScope.Complete();
			_transactionScope.Dispose();
		}

		public void Dispose()
		{
			_transactionScope.Dispose();
		}
	}
}