using System;
using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.DataAccess.Repositories.InMemory
{
	public class InMemoryAssignmentRepository : InMemoryRepositoryBase<Assignment>, IAssignmentRepository
	{
		public IEnumerable<Guid> GetRolesByAccount(Guid accountId)
		{
			return Storage.Where(x => x.AccountId.Equals(accountId)).Select(x => x.RoleId);
		}

		public void Assign(Guid accountId, Guid roleId)
		{
			Storage.Add(new Assignment
			{
				Id = Guid.NewGuid(),
				AccountId = accountId,
				RoleId = roleId
			});
		}

		public bool IsAccountAlreadyAssigned(Guid accountId, Guid roleId)
		{
			return Storage.Any(x => x.AccountId.Equals(accountId) && x.RoleId.Equals(roleId));
		}
	}
}