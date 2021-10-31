using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Postgres;
using Keebox.Common.DataAccess.Repositories.Postgres.Transactions;
using Keebox.Common.Exceptions;
using Keebox.Common.Types;

using NUnit.Framework;


namespace Keebox.Common.IntegrationTests.DataAccess.Repositories.Postgres
{
	[TestFixture]
	[Category("Integration")]
	public class PostgresGroupRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			var storageConnection = new StorageConnection
			{
				ConnectionString = ConfigurationHelper.PostgresDatabaseConnectionString
			};

			_transactionScopeFactory = new TransactionScopeFactory();
			_target = new PostgresGroupRepository(new ConnectionFactory(storageConnection));

			_transaction = _transactionScopeFactory.BeginTransaction();
		}

		[TearDown]
		public void TearDown()
		{
			_transaction.Dispose();
		}

		[Test]
		public void DeleteGroupTest()
		{
			// arrange
			var groupName = _fixture.Create<string>();
			var groupPath = _fixture.Create<string>();

			_target.CreateGroup(groupName, groupPath);

			// act
			var existing = _target.Get(groupName, groupPath);
			existing.Should().NotBeNull();

			_target.DeleteGroup(groupName, groupPath);

			// assert
			Assert.Throws<NotFoundException>(() => _target.Get(groupName, groupPath));
		}

		[Test]
		public void ExistsTest()
		{
			// arrange
			var groupName = _fixture.Create<string>();
			var groupPath = _fixture.Create<string>();

			_target.CreateGroup(groupName, groupPath);

			// act
			var result = _target.Exists(groupName, groupPath);

			// assert
			result.Should().BeTrue();
		}

		[Test]
		public void GetTest()
		{
			// arrange
			var groupName = _fixture.Create<string>();
			var groupPath = _fixture.Create<string>();

			_target.CreateGroup(groupName, groupPath);

			// act
			var result = _target.Get(groupName, groupPath);

			// assert
			result.Should().NotBeNull();

			result.Should().BeEquivalentTo(new Group
			{
				Name = groupName,
				Path = groupPath
			}, e => e.Excluding(x => x.Id));
		}

		private readonly IFixture _fixture = new Fixture();

		private ITransactionScope _transaction;
		private ITransactionScopeFactory _transactionScopeFactory;

		private PostgresGroupRepository _target;
	}
}