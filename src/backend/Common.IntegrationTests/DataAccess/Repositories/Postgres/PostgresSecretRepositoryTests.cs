using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Repositories.Postgres;
using Keebox.Common.DataAccess.Repositories.Postgres.Transactions;
using Keebox.Common.Types;

using NUnit.Framework;


namespace Keebox.Common.IntegrationTests.DataAccess.Repositories.Postgres
{
	[TestFixture]
	[Category("Integration")]
	public class PostgresSecretRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			var storageConnection = new StorageConnection
			{
				ConnectionString = ConfigurationHelper.PostgresDatabaseConnectionString
			};

			_transactionScopeFactory = new TransactionScopeFactory();
			_target = new PostgresSecretRepository(new ConnectionFactory(storageConnection));

			_transaction = _transactionScopeFactory.BeginTransaction();
		}

		[TearDown]
		public void TearDown()
		{
			_transaction.Dispose();
		}

		[Test]
		public void DeleteSecretsByGroupIdTest()
		{
			// arrange
			var secrets = _fixture.Create<Dictionary<string, string>>();
			var files = _fixture.Create<Dictionary<string, string>>();
			var groupId = _fixture.Create<Guid>();

			_target.SetGroupSecrets(groupId, secrets, files);

			var inserted = _target.GetGroupSecrets(groupId);
			inserted.Should().NotBeNull();

			// act
			_target.DeleteGroupSecrets(groupId, files.Select(x => x.Key));

			// assert
			var result = _target.GetGroupSecrets(groupId).ToArray();

			result.Length.Should().Be(secrets.Count);

			// act
			_target.DeleteGroupSecrets(groupId);

			// assert
			result = _target.GetGroupSecrets(groupId).ToArray();

			result.Should().BeEmpty();
		}

		[Test]
		public void GetGroupSecretsTest()
		{
			// arrange
			var secrets = _fixture.Create<Dictionary<string, string>>();
			var files = _fixture.Create<Dictionary<string, string>>();
			var groupId = _fixture.Create<Guid>();

			_target.SetGroupSecrets(groupId, secrets, files);

			// act
			var result = _target.GetGroupSecrets(groupId).ToList();

			// assert
			result.Should().NotBeNull();

			result.Select(x => x.Name).OrderBy(x => x).Should()
				.BeEquivalentTo(secrets.Keys.Concat(files.Keys).OrderBy(x => x));
		}

		[Test]
		public void UpdateGroupSecretsTest()
		{
			// arrange
			var secrets = _fixture.Create<Dictionary<string, string>>();
			var files = _fixture.Create<Dictionary<string, string>>();
			var groupId = _fixture.Create<Guid>();

			var newSecrets = secrets.ToDictionary(x => x.Key, _ => _fixture.Create<string>());
			var newFiles = files.ToDictionary(x => x.Key, _ => _fixture.Create<string>());

			_target.SetGroupSecrets(groupId, secrets, files);

			var inserted = _target.GetGroupSecrets(groupId);
			inserted.Should().NotBeNull();

			// act
			_target.UpdateGroupSecrets(groupId, newSecrets, newFiles);

			// assert
			var result = _target.GetGroupSecrets(groupId).ToList();

			result.Select(x => x.Name).OrderBy(x => x).Should()
				.BeEquivalentTo(newSecrets.Keys.Concat(files.Keys).OrderBy(x => x));
		}

		private readonly IFixture _fixture = new Fixture();

		private ITransactionScope _transaction;
		private ITransactionScopeFactory _transactionScopeFactory;

		private PostgresSecretRepository _target;
	}
}