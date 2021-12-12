using System;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<Guid> GetRolesByAccount(Guid accountId)
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Assignment>().Where(x => x.AccountId.Equals(accountId)).Select(x => x.RoleId).ToArray();
		}

		public void Assign(Guid accountId, Guid roleId)
		{
			using var connection = _connectionFactory.Create();

			connection.GetTable<Assignment>().Insert(() => new Assignment
			{
				Id = Guid.NewGuid(),
				AccountId = accountId,
				RoleId = roleId
			});
		}

		public bool IsAccountAlreadyAssigned(Guid accountId, Guid roleId)
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Assignment>()
				.SingleOrDefault(x => x.AccountId.Equals(accountId) && x.RoleId.Equals(roleId)) is not null;
		}

		private readonly IConnectionFactory _connectionFactory;
	}
}