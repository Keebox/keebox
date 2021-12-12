using System;

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
	public class PostgresAssignmentRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			var storageConnection = new StorageConnection
			{
				ConnectionString = ConfigurationHelper.PostgresDatabaseConnectionString
			};

			_transactionScopeFactory = new TransactionScopeFactory();
			_target = new PostgresAssignmentRepository(new ConnectionFactory(storageConnection));

			_transaction = _transactionScopeFactory.BeginTransaction();
		}

		[TearDown]
		public void TearDown()
		{
			_transaction.Dispose();
		}

		[Test]
		public void GetRolesByAccountTest()
		{
			// arrange
			var accountId = _fixture.Create<Guid>();
			var roleId = _fixture.Create<Guid>();

			_target.Assign(accountId, roleId);

			// act
			var result = _target.GetRolesByAccount(accountId);

			// assert
			result.Should().BeEquivalentTo(new[] { roleId });
		}

		[Test]
		public void IsAlreadyAssignedTest()
		{
			// arrange
			var accountId = _fixture.Create<Guid>();
			var roleId = _fixture.Create<Guid>();

			_target.Assign(accountId, roleId);

			// act
			var result = _target.IsAccountAlreadyAssigned(accountId, roleId);

			// assert
			result.Should().BeTrue();
		}

		private readonly Fixture _fixture = new();

		private ITransactionScope _transaction;
		private ITransactionScopeFactory _transactionScopeFactory;

		private PostgresAssignmentRepository _target;
	}
}