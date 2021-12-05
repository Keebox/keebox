using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;

using LinqToDB;
using LinqToDB.Data;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public class PostgresSecretRepository : ISecretRepository
	{
		public PostgresSecretRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public IEnumerable<Secret> GetGroupSecrets(Guid groupId)
		{
			EnsureArg.IsNotDefault(groupId);

			using var connection = _connectionFactory.Create();

			return connection.GetTable<Secret>().Where(x => x.GroupId == groupId).ToArray();
		}

		public void UpdateGroupSecrets(Guid groupId, Dictionary<string, string> secrets, Dictionary<string, string> files)
		{
			EnsureArg.IsNotDefault(groupId);
			EnsureArg.IsNotNull(secrets);
			EnsureArg.IsNotNull(files);

			void CreateOrUpdateSecret(DataConnection connection, string key, string value, bool isFile)
			{
				var table = connection.GetTable<Secret>();

				if (table.SingleOrDefault(x => x.Name.Equals(key)) is null)
				{
					table.Insert(() => new Secret
					{
						Name = key,
						Value = value,
						GroupId = groupId,
						IsFile = isFile
					});

					return;
				}

				connection.GetTable<Secret>().Where(x => x.Name.Equals(key) && x.GroupId == groupId)
					.Update(secret => new Secret
					{
						Id = secret.Id,
						Name = secret.Name,
						Value = value,
						GroupId = secret.GroupId,
						IsFile = isFile
					});
			}

			using var connection = _connectionFactory.Create();

			foreach (var (key, value) in secrets)
				CreateOrUpdateSecret(connection, key, value, false);

			foreach (var (key, value) in files)
				CreateOrUpdateSecret(connection, key, value, true);
		}

		public void SetGroupSecrets(Guid groupId, Dictionary<string, string> secrets, Dictionary<string, string> files)
		{
			EnsureArg.IsNotDefault(groupId);
			EnsureArg.IsNotNull(secrets);
			EnsureArg.IsNotNull(files);

			using var connection = _connectionFactory.Create();

			connection.GetTable<Secret>().Delete(x => x.GroupId == groupId);

			foreach (var (key, value) in secrets)
				connection.GetTable<Secret>().Insert(() => new Secret
				{
					Name = key,
					Value = value,
					GroupId = groupId,
					IsFile = false
				});

			foreach (var (key, value) in files)
				connection.GetTable<Secret>().Insert(() => new Secret
				{
					Name = key,
					Value = value,
					GroupId = groupId,
					IsFile = true
				});
		}

		public void DeleteGroupSecrets(Guid groupId, IEnumerable<string> secrets)
		{
			EnsureArg.IsNotDefault(groupId);

			using var connection = _connectionFactory.Create();

			foreach (var secret in secrets)
				connection.GetTable<Secret>().Delete(x => x.GroupId == groupId && x.Name.Equals(secret));
		}

		public void DeleteGroupSecrets(Guid groupId)
		{
			using var connection = _connectionFactory.Create();

			connection.GetTable<Secret>().Delete(x => x.GroupId == groupId);
		}

		private readonly IConnectionFactory _connectionFactory;
	}
}