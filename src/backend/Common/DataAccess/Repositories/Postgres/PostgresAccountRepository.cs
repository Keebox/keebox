﻿using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using LinqToDB;

using Ralfred.Common.DataAccess.Entities;
using Ralfred.Common.DataAccess.Repositories.Abstractions;


namespace Ralfred.Common.DataAccess.Repositories.Postgres
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

			return connection.GetTable<Account>().SingleOrDefault(x => x.Name != null && x.Name.Equals(accountName)) is not null;
		}

		public bool ExistsWithToken(string tokenHash)
		{
			EnsureArg.IsNotNullOrWhiteSpace(tokenHash);

			using var connection = _connectionFactory.Create();

			return connection.GetTable<Account>().SingleOrDefault(x => x.TokenHash != null && x.TokenHash.Equals(tokenHash)) is not null;

			;
		}

		public Guid Create(Account account)
		{
			if (string.IsNullOrEmpty(account.Name))
			{
				EnsureArg.IsNotNullOrWhiteSpace(account.TokenHash);
			}

			if (account.Id == Guid.Empty)
			{
				account.Id = Guid.NewGuid();
			}

			using var connection = _connectionFactory.Create();

			connection.GetTable<Account>().Insert(() => new Account
			{
				Id = account.Id == Guid.Empty ? Guid.NewGuid() : account.Id,
				Name = account.Name,
				CertificateThumbprint = account.CertificateThumbprint,
				TokenHash = account.TokenHash
			});

			return account.Id;
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

			return connection.GetTable<Account>().Single(x => x.Name != null && x.Name.Equals(accountName));
		}

		public Account Update(Account account)
		{
			if (string.IsNullOrEmpty(account.Name))
			{
				EnsureArg.IsNotNullOrWhiteSpace(account.TokenHash);
			}

			using var connection = _connectionFactory.Create();

			connection.Update(account);

			return account;
		}

		public IEnumerable<Account> List()
		{
			using var connection = _connectionFactory.Create();

			return connection.GetTable<Account>().ToArray();
		}

		private readonly IConnectionFactory _connectionFactory;
	}
}