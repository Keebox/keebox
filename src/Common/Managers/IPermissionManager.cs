using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.Managers
{
	public interface IPermissionManager
	{
		IEnumerable<Permission> GetGroupPermissions(Guid groupId);

		Permission GetPermission(Guid permissionId);

		Guid CreatePermission(Guid roleId, Guid groupId, bool isReadOnly);

		void DeletePermission(Guid permissionId);

		void UpdatePermission(Permission permission);
	}
}