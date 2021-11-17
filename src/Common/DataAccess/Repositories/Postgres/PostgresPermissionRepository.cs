using System;
using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;

using LinqToDB;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public class PostgresPermissionRepository : IPermissionRepository
	{
		public PostgresPermissionRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public IEnumerable<Permission> GetByGroupId(Guid groupId)
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Permission>().Where(x => x.GroupId == groupId).ToArray();
		}

		public bool Exists(Guid permissionId)
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Permission>().Any(x => x.Id == permissionId);
		}

		public Permission Get(Guid permissionId)
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Permission>().Single(x => x.Id == permissionId);
		}

		public Guid Create(Permission permission)
		{
			using var connection = _connectionFactory.Create();

			var permissionId = permission.Id == default ? Guid.NewGuid() : permission.Id;
			connection.GetTable<Permission>().Insert(() => new Permission
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
			using var connection = _connectionFactory.Create();

			connection.GetTable<Permission>().Delete(x => x.Id == permissionId);
		}

		public void Update(Permission permission)
		{
			using var connection = _connectionFactory.Create();

			connection.Update(permission);
		}

		private readonly IConnectionFactory _connectionFactory;
	}
}