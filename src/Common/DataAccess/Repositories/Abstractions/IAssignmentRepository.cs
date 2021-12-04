using System;
using System.Collections.Generic;


namespace Keebox.Common.DataAccess.Repositories.Abstractions
{
	public interface IAssignmentRepository
	{
		public IEnumerable<Guid> GetByAccount(Guid accountId);

		public void Assign(Guid accountId, Guid roleId);
	}
}