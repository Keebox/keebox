using System;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Repositories.InMemory;

using NUnit.Framework;


namespace Keebox.Common.IntegrationTests.DataAccess.Repositories.InMemory
{
	[TestFixture]
	[Category("Integration")]
	public class InMemoryAssignmentRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			_target = new InMemoryAssignmentRepository();
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

		private InMemoryAssignmentRepository _target;
	}
}