using System;
using System.Collections.Generic;


namespace Keebox.Common.DataAccess.Repositories.Abstractions
{
	public interface IAssignmentRepository
	{
		IEnumerable<Guid> GetRolesByAccount(Guid accountId);

		void Assign(Guid accountId, Guid roleId);

		bool IsAccountAlreadyAssigned(Guid accountId, Guid roleId);
	}
}