using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.InMemory;
using Keebox.Common.DataAccess.Repositories.Postgres;
using Keebox.Common.DataAccess.Repositories.Postgres.Transactions;
using Keebox.Common.IntegrationTests.Helpers;
using Keebox.Common.Types;

using NUnit.Framework;


namespace Keebox.Common.IntegrationTests.DataAccess.Repositories.InMemory
{
	[TestFixture]
	[Category("Integration")]
	public class InMemoryRoleRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			_target = new InMemoryRoleRepository();
		}

		[Test]
		public void DeleteTest()
		{
			// arrange
			var name = GenerateName();

			var id = _target.Create(name);

			// act
			_target.Delete(id);

			// assert
			_target.List().Count(x => x.Id == id).Should().Be(0);
		}

		[Test]
		public void ListTest()
		{
			// arrange
			var count = _fixture.Create<int>();
			var names = new List<string>();

			for (var i = 0; i < count; i++)
			{
				var name = GenerateName();
				_target.Create(name);
				names.Add(name);
			}

			// act
			var result = _target.List().ToArray();

			// assert
			result.Length.Should().Be(count);
			result.Select(x => x.Name).Should().BeEquivalentTo(names);
		}

		[Test]
		public void UpdateTest()
		{
			// arrange
			var name = GenerateName();

			var id = _target.Create(name);

			var newName = GenerateName();

			// act
			_target.Update(new Role
			{
				Id = id,
				Name = newName
			});

			// assert
			var list = _target.List().ToArray();

			list.Count(x => x.Name == name).Should().Be(0);
			list.Count(x => x.Name == newName).Should().Be(1);
		}
		
		
		[Test]
		public void GetTest()
		{
			// arrange
			var name = GenerateName();

			var id = _target.Create(name);
			
			// act
			var role = _target.Get(id);
			
			// assert
			role.Name.Should().Be(name);
		}

		private string GenerateName() => Creator.CreateStringWithMaxLength(_fixture.Create<int>() % 256);

		private readonly IFixture _fixture = new Fixture();

		private InMemoryRoleRepository _target;
	}
}