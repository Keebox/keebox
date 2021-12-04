using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Postgres;
using Keebox.Common.DataAccess.Repositories.Postgres.Transactions;
using Keebox.Common.IntegrationTests.Helpers;
using Keebox.Common.Types;

using NUnit.Framework;


namespace Keebox.Common.IntegrationTests.DataAccess.Repositories.Postgres
{
	[TestFixture]
	[Category("Integration")]
	public class PostgresAccountRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			var storageConnection = new StorageConnection
			{
				ConnectionString = ConfigurationHelper.PostgresDatabaseConnectionString
			};

			_transactionScopeFactory = new TransactionScopeFactory();
			_target = new PostgresAccountRepository(new ConnectionFactory(storageConnection));

			_transaction = _transactionScopeFactory.BeginTransaction();
		}

		[TearDown]
		public void TearDown()
		{
			_transaction.Dispose();
		}

		[Test]
		public void DeleteTest()
		{
			// arrange
			var account = CreateAccount();

			var id = _target.Create(account);

			// act
			_target.Delete(id);

			// assert
			_target.List().Should().HaveCount(0);
		}

		[Test]
		public void Exists_ForNotExistingAccountTest()
		{
			// arrange
			var accountName = Creator.CreateStringWithMaxLength(255);

			// act
			var result = _target.Exists(accountName);

			// assert
			result.Should().BeFalse();
		}

		[Test]
		public void Exists_ByNameTest()
		{
			// arrange
			var account = CreateAccount();

			var id = _target.Create(account);

			// act
			var result = _target.Exists(account.Name!);

			// assert
			result.Should().BeTrue();
			id.Should().NotBe(Guid.Empty);
		}

		[Test]
		public void Exists_ByIdTest()
		{
			// arrange
			var account = CreateAccount();

			var id = _target.Create(account);

			// act
			var result = _target.Exists(id);

			// assert
			result.Should().BeTrue();
			id.Should().NotBe(Guid.Empty);
		}

		[Test]
		public void GetByNameTest()
		{
			// arrange
			var account = CreateAccount();

			var id = _target.Create(account);

			// act
			var result = _target.GetByName(account.Name!);

			// assert
			result.Should().NotBeNull();
			id.Should().NotBe(Guid.Empty);
		}

		[Test]
		public void ListTest()
		{
			// arrange
			var ids = new List<Guid>();
			var accountsCount = _fixture.Create<Generator<int>>().First(x => x > 0 && x < 10);

			for (var i = 0; i < accountsCount; i++)
			{
				var account = CreateAccount();
				ids.Add(_target.Create(account));
			}

			// act
			var accounts = _target.List().ToArray();

			// assert
			accounts.Should().HaveCount(accountsCount);
			accounts.Select(a => a.Id).Should().Equal(ids);
		}

		[Test]
		public void UpdateTest()
		{
			// arrange
			var account = CreateAccount();
			var tokenHashLength = _fixture.Create<Generator<int>>().First(x => x < 64);

			var id = _target.Create(account);

			account.TokenHash = Creator.CreateStringWithMaxLength(tokenHashLength);

			// act
			_target.Update(account);

			// assert
			var updated = _target.GetByName(account.Name!);

			updated.Should().NotBeNull();
			updated.Should().BeEquivalentTo(account);
			id.Should().NotBe(Guid.Empty);
		}

		[Test]
		public void GetTest()
		{
			// arrange
			var account = CreateAccount();

			var id = _target.Create(account);

			// act
			var savedAccount = _target.Get(id);

			// assert
			savedAccount.Should().BeEquivalentTo(account, options => options.Excluding(x => x.Id));
		}

		private Account CreateAccount()
		{
			var nameLength = _fixture.Create<Generator<int>>().First(x => x < 255);
			var tokenHashLength = _fixture.Create<Generator<int>>().First(x => x < 64);
			var certificateThumbprintLength = _fixture.Create<Generator<int>>().First(x => x < 40);

			var account = _fixture.Build<Account>()
				.With(x => x.Name, Creator.CreateStringWithMaxLength(nameLength))
				.With(x => x.TokenHash, Creator.CreateStringWithMaxLength(tokenHashLength))
				.With(x => x.CertificateThumbprint, Creator.CreateStringWithMaxLength(certificateThumbprintLength))
				.Create();

			return account;
		}

		private readonly IFixture _fixture = new Fixture();

		private ITransactionScope _transaction;
		private ITransactionScopeFactory _transactionScopeFactory;

		private PostgresAccountRepository _target;
	}
}