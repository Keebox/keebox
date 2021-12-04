using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;

using LinqToDB;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public class PostgresAssignmentRepository : IAssignmentRepository
	{
		public PostgresAssignmentRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public IEnumerable<Guid> GetByAccount(Guid accountId)
		{
			EnsureArg.IsNotDefault(accountId);

			var connection = _connectionFactory.Create();

			return connection.GetTable<Assignment>().Where(x => x.AccountId.Equals(accountId)).Select(x => x.RoleId);
		}

		public void Assign(Guid accountId, Guid roleId)
		{
			EnsureArg.IsNotDefault(accountId);
			EnsureArg.IsNotDefault(roleId);

			var connection = _connectionFactory.Create();

			connection.GetTable<Assignment>().Insert(() => new Assignment
			{
				Id = Guid.NewGuid(),
				AccountId = accountId,
				RoleId = roleId
			});
		}

		private readonly IConnectionFactory _connectionFactory;
	}
}