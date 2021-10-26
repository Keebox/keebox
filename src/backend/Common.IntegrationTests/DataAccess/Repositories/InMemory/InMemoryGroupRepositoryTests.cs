﻿using AutoFixture;

using FluentAssertions;

using NUnit.Framework;

using Ralfred.Common.DataAccess.Entities;
using Ralfred.Common.DataAccess.Repositories.InMemory;


namespace Common.IntegrationTests.DataAccess.Repositories.InMemory
{
	[TestFixture]
	[Category("Integration")]
	public class InMemoryGroupRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			_target = new InMemoryGroupRepository();
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

			result.Should().BeEquivalentTo(new Group()
			{
				Name = groupName,
				Path = groupPath
			}, e => e.Excluding(x => x.Id));
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
			var deleted = _target.Get(groupName, groupPath);
			deleted.Should().BeNull();
		}

		private IFixture _fixture = new Fixture();

		private InMemoryGroupRepository _target;
	}
}