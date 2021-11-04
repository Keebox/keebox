using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Repositories.InMemory;

using NUnit.Framework;


namespace Keebox.Common.IntegrationTests.DataAccess.Repositories.InMemory
{
	[TestFixture]
	[Category("Integration")]
	public class InMemorySecretRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			_target = new InMemorySecretRepository();
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

		private InMemorySecretRepository _target;
	}
}