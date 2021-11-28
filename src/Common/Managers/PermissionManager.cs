using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;


namespace Keebox.Common.Managers
{
	public class PermissionManager : IPermissionManager
	{
		public PermissionManager(IRepositoryContext repositoryContext)
		{
			_permissionRepository = repositoryContext.GetPermissionRepository();
			_groupRepository = repositoryContext.GetGroupRepository();
			_roleRepository = repositoryContext.GetRoleRepository();
		}

		public IEnumerable<Permission> GetGroupPermissions(Guid groupId)
		{
			if (!_groupRepository.Exists(groupId))
			{
				throw new NotFoundException("Group not found");
			}

			return _permissionRepository.GetByGroupId(groupId);
		}

		public Permission GetPermission(Guid permissionId)
		{
			EnsurePermission(permissionId);

			return _permissionRepository.Get(permissionId);
		}

		public Guid CreatePermission(Guid roleId, Guid groupId, bool isReadOnly)
		{
			if (!_groupRepository.Exists(groupId))
			{
				throw new NotFoundException("Group not found");
			}

			if (!_roleRepository.Exists(roleId))
			{
				throw new NotFoundException("Role not found");
			}

			return _permissionRepository.Create(new Permission
			{
				GroupId = groupId,
				RoleId = roleId,
				IsReadOnly = isReadOnly
			});
		}

		public void DeletePermission(Guid permissionId)
		{
			EnsurePermission(permissionId);

			_permissionRepository.Delete(permissionId);
		}

		public void UpdatePermission(Permission permission)
		{
			EnsurePermission(permission.Id);

			_permissionRepository.Update(permission);
		}

		private void EnsurePermission(Guid permissionId)
		{
			if (!_permissionRepository.Exists(permissionId))
			{
				throw new NotFoundException("Permission not found");
			}
		}

		private readonly IPermissionRepository _permissionRepository;
		private readonly IGroupRepository _groupRepository;
		private readonly IRoleRepository _roleRepository;
	}
}