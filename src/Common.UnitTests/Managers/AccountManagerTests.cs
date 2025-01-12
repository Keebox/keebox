﻿using System;
using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Managers;
using Keebox.Common.Security;

using Moq;

using NUnit.Framework;


namespace Keebox.Common.UnitTests.Managers
{
	[TestFixture]
	public class AccountManagerTests
	{
		[SetUp]
		public void Setup()
		{
			_accountRepository = new Mock<IAccountRepository>();
			_repositoryContext = new Mock<IRepositoryContext>();
			_cryptoService = new Mock<ICryptoService>();

			_repositoryContext.Setup(x => x.GetAccountRepository()).Returns(_accountRepository.Object);

			_target = new AccountManager(_repositoryContext.Object, _cryptoService.Object);
		}

		[Test]
		public void CreateTokenAccountTest()
		{
			// arrange
			var token = _fixture.Create<string>();
			var name = _fixture.Create<string>();
			var tokenHash = _fixture.Create<string>();

			_cryptoService.Setup(x => x.GetHash(It.Is<string>(y => y.Equals(token)))).Returns(tokenHash).Verifiable();
			_accountRepository.Setup(x => x.Create(It.Is<Account>(y => y.TokenHash.Equals(tokenHash))));

			// act
			_target.CreateTokenAccount(name, token);

			// assert
			_cryptoService.Verify(x => x.GetHash(It.Is<string>(y => y.Equals(token))), Times.Once);
			_accountRepository.Verify(x => x.Create(It.Is<Account>(y => y.TokenHash.Equals(tokenHash) && y.Name.Equals(name))), Times.Once);
		}

		[Test]
		public void DeleteAccountTest()
		{
			// arrange
			var id = _fixture.Create<Guid>();

			_accountRepository.Setup(x => x.Exists(It.Is<Guid>(y => y == id))).Returns(true);
			_accountRepository.Setup(x => x.Delete(It.Is<Guid>(y => y == id))).Verifiable();

			// act
			_target.DeleteAccount(id);

			// assert
			_accountRepository.Verify(x => x.Delete(It.Is<Guid>(y => y == id)), Times.Once);
		}

		[Test]
		public void GetAccountsTest()
		{
			// arrange
			var accounts = _fixture.CreateMany<Account>().ToList();

			_accountRepository.Setup(x => x.List()).Returns(accounts).Verifiable();

			// act
			var gotAccounts = _target.GetAccounts();

			// arrange
			_accountRepository.Verify(x => x.List(), Times.Once);
			gotAccounts.Should().Equal(accounts);
		}

		private readonly Fixture _fixture = new();

		private Mock<ICryptoService> _cryptoService;
		private Mock<IRepositoryContext> _repositoryContext;
		private Mock<IAccountRepository> _accountRepository;

		private IAccountManager _target;
	}
}