using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.InMemory;

using NUnit.Framework;


namespace Keebox.Common.IntegrationTests.DataAccess.Repositories.InMemory
{
	[TestFixture]
	[Category("Integration")]
	public class InMemoryAccountRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			_target = new InMemoryAccountRepository();
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
			var accountName = _fixture.Create<string>();

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

			for (var i = 0; i < 10; i++)
			{
				var account = CreateAccount();
				ids.Add(_target.Create(account));
			}

			// act
			var accounts = _target.List().ToArray();

			// assert
			accounts.Should().HaveCount(accounts.Length);
			accounts.Select(a => a.Id).Should().Equal(ids);
		}

		[Test]
		public void UpdateTest()
		{
			// arrange
			var account = CreateAccount();

			var id = _target.Create(account);

			account.TokenHash = _fixture.Create<string>();

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
			var account = _fixture.Build<Account>()
				.With(x => x.Name, _fixture.Create<string>())
				.With(x => x.TokenHash, _fixture.Create<string>())
				.With(x => x.CertificateThumbprint, _fixture.Create<string>())
				.Create();

			return account;
		}

		private readonly IFixture _fixture = new Fixture();

		private InMemoryAccountRepository _target;
	}
}