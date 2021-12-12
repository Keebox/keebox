using System;
using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.DataAccess.Repositories.InMemory
{
	public class InMemoryPermissionRepository : InMemoryRepositoryBase<Permission>, IPermissionRepository
	{
		public IEnumerable<Permission> GetByGroupId(Guid groupId)
		{
			return Storage.Where(x => x.GroupId == groupId);
		}

		public bool Exists(Guid permissionId)
		{
			return Storage.Any(x => x.Id == permissionId);
		}

		public Permission Get(Guid permissionId)
		{
			return Storage.Single(x => x.Id == permissionId);
		}

		public Guid Create(Permission permission)
		{
			var permissionId = permission.Id == default ? Guid.NewGuid() : permission.Id;

			Storage.Add(new Permission
			{
				Id = permissionId,
				GroupId = permission.GroupId,
				RoleId = permission.RoleId,
				IsReadOnly = permission.IsReadOnly
			});

			return permissionId;
		}

		public void Delete(Guid permissionId)
		{
			Storage.RemoveAll(x => x.Id == permissionId);
		}

		public void Update(Permission permission)
		{
			var index = Storage.FindIndex(x => x.Id == permission.Id);

			if (index == -1)
			{
				return;
			}

			Storage[index] = permission;
		}
	}
}