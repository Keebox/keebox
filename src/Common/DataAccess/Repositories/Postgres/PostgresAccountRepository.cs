using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;

using LinqToDB;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public class PostgresAccountRepository : IAccountRepository
	{
		public PostgresAccountRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public bool Exists(string accountName)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(accountName);

			using var connection = _connectionFactory.Create();

			return connection.GetTable<Account>().SingleOrDefault(x => x.Name.Equals(accountName)) is not null;
		}

		public bool Exists(Guid accountId)
		{
			var connection = _connectionFactory.Create();

			return connection.GetTable<Account>().Any(x => x.Id == accountId);
		}

		public bool ExistsWithToken(string tokenHash)
		{
			EnsureArg.IsNotNullOrWhiteSpace(tokenHash);

			using var connection = _connectionFactory.Create();

			return connection.GetTable<Account>()
				.SingleOrDefault(x => x.TokenHash != null && x.TokenHash.Equals(tokenHash)) is not null;
		}

		public Guid Create(Account account)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(account.Name);

			using var connection = _connectionFactory.Create();

			// NOTE: InsertWithOutput is not yet supported for PostreSQL https://github.com/linq2db/linq2db/issues/2958
			var accountId = account.Id == default ? Guid.NewGuid() : account.Id;

			connection.GetTable<Account>().Insert(() => new Account
			{
				Id = accountId,
				Name = account.Name,
				TokenHash = account.TokenHash,
				CertificateThumbprint = account.CertificateThumbprint,
			});

			return accountId;
		}

		public void Delete(Guid accountId)
		{
			using var connection = _connectionFactory.Create();
			connection.GetTable<Account>().Delete(x => x.Id == accountId);
		}

		public Account GetByName(string accountName)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(accountName);

			using var connection = _connectionFactory.Create();

			return connection.GetTable<Account>().Single(x => x.Name.Equals(accountName));
		}

		public Account GetByTokenHash(string tokenHash)
		{
			EnsureArg.IsNotNullOrWhiteSpace(tokenHash);

			using var connection = _connectionFactory.Create();

			return connection.GetTable<Account>().Single(x => x.TokenHash != null && x.TokenHash.Equals(tokenHash));
		}

		public void Update(Account account)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(account.Name);

			using var connection = _connectionFactory.Create();

			connection.Update(account);
		}

		public IEnumerable<Account> List()
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Account>().ToArray();
		}

		public Account Get(Guid accountId)
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Account>().Single(x => x.Id == accountId);
		}

		private readonly IConnectionFactory _connectionFactory;
	}
}