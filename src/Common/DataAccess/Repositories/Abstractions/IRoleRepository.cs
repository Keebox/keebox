using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.Abstractions
{
	public interface IRoleRepository
	{
		IEnumerable<Role> List();

		Role Create(Role role);

		void Update(Role role);

		void Delete(Guid roleId);
	}
}