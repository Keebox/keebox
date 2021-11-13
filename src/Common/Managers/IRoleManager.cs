using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.Managers
{
	public interface IRoleManager
	{
		IEnumerable<Role> GetRoles();

		Guid CreateRole(string name);

		void UpdateRole(Role role);

		void DeleteRole(Guid roleId);

		Role GetRole(Guid roleId);
	}
}