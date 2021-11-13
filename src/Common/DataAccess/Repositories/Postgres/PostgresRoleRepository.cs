﻿using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;

using LinqToDB;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public class PostgresRoleRepository : IRoleRepository
	{
		public PostgresRoleRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public IEnumerable<Role> List()
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Role>().ToArray();
		}

		public Guid Create(string name)
		{
			EnsureArg.IsNotNullOrWhiteSpace(name);

			using var connection = _connectionFactory.Create();

			var roleId = Guid.NewGuid();
			connection.GetTable<Role>().Insert(() => new Role
			{
				Id = roleId,
				Name = name
			});

			return roleId;
		}

		public void Update(Role role)
		{
			EnsureArg.IsNotNullOrWhiteSpace(role.Name);

			using var connection = _connectionFactory.Create();

			connection.GetTable<Role>().Update(_ => new Role
			{
				Id = role.Id,
				Name = role.Name
			});
		}

		public void Delete(Guid roleId)
		{
			using var connection = _connectionFactory.Create();

			connection.GetTable<Role>().Delete(x => x.Id == roleId);
		}

		private readonly IConnectionFactory _connectionFactory;
	}
}