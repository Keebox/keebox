using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.Abstractions
{
	public interface IPermissionRepository
	{
		IEnumerable<Permission> GetByGroupId(Guid groupId);

		bool Exists(Guid permissionId);

		Permission Get(Guid permissionId);

		Guid Create(Permission permission);

		void Delete(Guid permissionId);

		void Update(Permission permission);
	}
}