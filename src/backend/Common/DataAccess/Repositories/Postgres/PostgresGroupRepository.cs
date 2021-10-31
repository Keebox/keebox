using System;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;

using LinqToDB;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public class PostgresGroupRepository : IGroupRepository
	{
		private readonly IConnectionFactory _connectionFactory;

		public PostgresGroupRepository(IConnectionFactory connectionFactory) =>
			_connectionFactory = connectionFactory;

		public bool Exists(string name, string path)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(name);

			using var connection = _connectionFactory.Create();

			return connection.GetTable<Group>().SingleOrDefault(x => x.Name.Equals(name) && x.Path.Equals(path)) is not null;
		}

		public Group Get(string name, string path)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(name);

			using var connection = _connectionFactory.Create();

			try
			{
				return connection.GetTable<Group>().Single(x => x.Name.Equals(name) && x.Path.Equals(path));
			}
			catch (InvalidOperationException)
			{
				throw new NotFoundException("Group not found");
			}
		}

		public Guid CreateGroup(string name, string path)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(name);

			using var connection = _connectionFactory.Create();

			var group = new Group
			{
				Name = name,
				Path = path
			};

			LinqExtensions.Insert(connection.GetTable<Group>(), () => new Group
			{
				Name = name,
				Path = path
			});

			return group.Id;
		}

		public void DeleteGroup(string name, string path)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(name);

			using var connection = _connectionFactory.Create();

			LinqExtensions.Delete(connection.GetTable<Group>(), x => x.Name.Equals(name) && x.Path.Equals(path));
		}
	}
}