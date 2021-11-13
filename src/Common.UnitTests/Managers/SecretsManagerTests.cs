using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;
using Keebox.Common.Helpers;
using Keebox.Common.Managers;
using Keebox.Common.Types;

using Moq;

using NUnit.Framework;


namespace Keebox.Common.UnitTests.Managers
{
	[TestFixture]
	public class SecretsManagerTests
	{
		[SetUp]
		public void Setup()
		{
			_pathResolver = new Mock<IPathResolver>();

			_repositoryContext = new Mock<IRepositoryContext>();
			_secretsRepository = new Mock<ISecretsRepository>();
			_groupRepository = new Mock<IGroupRepository>();

			_repositoryContext.Setup(x => x.GetGroupRepository()).Returns(_groupRepository.Object);
			_repositoryContext.Setup(x => x.GetSecretRepository()).Returns(_secretsRepository.Object);

			_target = new SecretManager(_pathResolver.Object, _repositoryContext.Object);
		}

		private readonly IFixture _fixture = new Fixture();

		private ISecretManager _target;
		private Mock<IPathResolver> _pathResolver;
		private Mock<ISecretsRepository> _secretsRepository;
		private Mock<IGroupRepository> _groupRepository;
		private Mock<IRepositoryContext> _repositoryContext;

		[Test]
		public void AddSecretFileTest()
		{

			// arrange
			const string fullPath = "path/to/group/secret";
			const string secretName = "secret";
			const string groupPath = "path/to/group";
			const string groupName = "group";
			const string folderPath = "path/to";

			var group = _fixture.Create<Group>();

			var files = new Dictionary<string, string> { { "value", "file" } };

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Secret);
			_pathResolver.Setup(x => x.DeconstructPath(fullPath)).Returns((secretName, groupPath));
			_pathResolver.Setup(x => x.DeconstructPath(groupPath)).Returns((groupName, folderPath));

			_groupRepository.Setup(x => x.Get(groupName, folderPath)).Returns(group);

			_secretsRepository.Setup(x => x.UpdateGroupSecrets(group.Id,
					It.Is<Dictionary<string, string>>(y => y.Keys.Count == 0),
					It.Is<Dictionary<string, string>>(y => y.Keys.Count == 1 && y.ContainsKey(secretName))))
				.Verifiable();

			// act
			_target.AddSecrets(fullPath, new Dictionary<string, string>(), files, Array.Empty<string>());

			// assert
			_secretsRepository.Verify(x => x.UpdateGroupSecrets(group.Id,
				It.Is<Dictionary<string, string>>(y => y.Keys.Count == 0),
				It.Is<Dictionary<string, string>>(y => y.Keys.Count == 1 && y.ContainsKey(secretName))), Times.Once);
		}

		[Test]
		public void AddSecretsGroupTest()
		{

			// arrange
			const string fullPath = "path/to/group";
			const string name = "group";
			const string path = "path/to";

			var group = _fixture.Create<Group>();

			var secrets = new Dictionary<string, string> { { "test", "test" } };
			var files = new Dictionary<string, string> { { "file", "file" } };

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Group);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));
			_groupRepository.Setup(x => x.Get(name, path)).Returns(group);
			_secretsRepository.Setup(x => x.SetGroupSecrets(group.Id, secrets, files)).Verifiable();

			// act
			_target.AddSecrets(fullPath, secrets, files, new string[] { });

			// assert
			_secretsRepository.Verify(x => x.SetGroupSecrets(group.Id, secrets, files), Times.Once);
		}

		[Test]
		public void AddSecretsGroupWithNamesTest()
		{
			// arrange
			const string fullPath = "path/to/group";
			const string name = "group";
			const string path = "path/to";

			var group = _fixture.Create<Group>();

			var secrets = new Dictionary<string, string> { { "test", "test" }, { "test2", "test2" } };
			var files = new Dictionary<string, string> { { "file", "file" } };

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Group);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));
			_groupRepository.Setup(x => x.Get(name, path)).Returns(group);

			_secretsRepository.Setup(x => x.UpdateGroupSecrets(group.Id,
					It.Is<Dictionary<string, string>>(y => y.Keys.Any() && y.ContainsKey("test")),
					It.Is<Dictionary<string, string>>(y => y.Keys.Count == 0)))
				.Verifiable();

			// act
			_target.AddSecrets(fullPath, secrets, files, new[] { "test" });

			// assert
			_secretsRepository.Verify(
				x => x.UpdateGroupSecrets(group.Id,
					It.Is<Dictionary<string, string>>(y => y.Keys.Any() && y.ContainsKey("test")),
					It.Is<Dictionary<string, string>>(y => y.Keys.Count == 0)),
				Times.Once);
		}

		[Test]
		public void AddSecretsTest()
		{
			// arrange
			const string fullPath = "path/to/group";
			const string name = "group";
			const string path = "path/to";
			var groupId = Guid.Empty;

			var secrets = new Dictionary<string, string> { { "test", "test" } };
			var files = new Dictionary<string, string> { { "file", "file" } };

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.None);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));
			_groupRepository.Setup(x => x.CreateGroup(name, path)).Returns(groupId);
			_secretsRepository.Setup(x => x.SetGroupSecrets(groupId, secrets, files)).Verifiable();

			// act
			_target.AddSecrets(fullPath, secrets, files, new string[] { });

			// assert
			_groupRepository.Verify(x => x.CreateGroup(name, path), Times.Once);
			_pathResolver.Verify(x => x.Resolve(It.IsAny<string>()), Times.Once);
			_pathResolver.Verify(x => x.DeconstructPath(It.IsAny<string>()), Times.Once);
			_secretsRepository.Verify(x => x.SetGroupSecrets(groupId, secrets, files), Times.Once);
		}

		[Test]
		public void AddSecretsWithNamesTest()
		{
			// arrange
			const string fullPath = "path/to/group";
			const string name = "group";
			const string path = "path/to";
			var groupId = Guid.Empty;

			var secrets = new Dictionary<string, string> { { "test", "test" }, { "test2", "test2" } };
			var files = new Dictionary<string, string> { { "file", "file" } };

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.None);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));

			_groupRepository.Setup(x => x.CreateGroup(name, path)).Returns(groupId);

			_secretsRepository.Setup(x =>
				x.SetGroupSecrets(groupId, It.Is<Dictionary<string, string>>(y => y.Keys.Any() && y.ContainsKey("test")),
					It.Is<Dictionary<string, string>>(y => y.Keys.Count == 0)));

			// act
			_target.AddSecrets(fullPath, secrets, files, new[] { "test" });

			// assert
			_groupRepository.Verify(x => x.CreateGroup(name, path), Times.Once);

			_secretsRepository.Verify(x =>
				x.SetGroupSecrets(groupId, It.Is<Dictionary<string, string>>(y => y.Keys.Any() && y.ContainsKey("test")),
					It.Is<Dictionary<string, string>>(y => y.Keys.Count == 0)));

			_pathResolver.Verify(x => x.Resolve(It.IsAny<string>()));
			_pathResolver.Verify(x => x.DeconstructPath(It.IsAny<string>()));
		}

		[Test]
		public void AddSecretTest()
		{

			// arrange
			const string fullPath = "path/to/group/secret";
			const string secretName = "secret";
			const string groupPath = "path/to/group";
			const string groupName = "group";
			const string folderPath = "path/to";

			var group = _fixture.Create<Group>();

			var secrets = new Dictionary<string, string> { { "value", "file" } };

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Secret);
			_pathResolver.Setup(x => x.DeconstructPath(fullPath)).Returns((secretName, groupPath));
			_pathResolver.Setup(x => x.DeconstructPath(groupPath)).Returns((groupName, folderPath));

			_groupRepository.Setup(x => x.Get(groupName, folderPath)).Returns(group);

			_secretsRepository.Setup(x => x.UpdateGroupSecrets(group.Id,
					It.Is<Dictionary<string, string>>(y => y.Keys.Count == 1 && y.ContainsKey(secretName)),
					It.Is<Dictionary<string, string>>(y => y.Keys.Count == 0)))
				.Verifiable();

			// act
			_target.AddSecrets(fullPath, secrets, new Dictionary<string, string>(), Array.Empty<string>());

			// assert
			_secretsRepository.Verify(x => x.UpdateGroupSecrets(group.Id,
				It.Is<Dictionary<string, string>>(y => y.Keys.Count == 1 && y.ContainsKey(secretName)),
				It.Is<Dictionary<string, string>>(y => y.Keys.Count == 0)), Times.Once);
		}

		[Test]
		public void AddSecretWithoutValueTest()
		{

			// arrange
			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Secret);

			// assert
			Assert.Throws<ArgumentException>(() => _target.AddSecrets("path/to/folder", new Dictionary<string, string>(),
				new Dictionary<string, string>(), Array.Empty<string>()));
		}

		[Test]
		public void DeleteGroupSecretsTest()
		{
			// arrange
			const string fullPath = "path/to/group";
			const string path = "path/to";
			const string name = "group";
			var secrets = new[] { "test" };

			var group = _fixture.Create<Group>();

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Group);
			_pathResolver.Setup(x => x.DeconstructPath(fullPath)).Returns((name, path));
			_groupRepository.Setup(x => x.Get(name, path)).Returns(group);
			_secretsRepository.Setup(x => x.DeleteGroupSecrets(group.Id, secrets)).Verifiable();

			// act
			_target.DeleteSecrets(fullPath, secrets);

			// assert
			_secretsRepository.Verify(x => x.DeleteGroupSecrets(group.Id, secrets), Times.Once);
		}

		[Test]
		public void DeleteGroupTest()
		{
			// arrange
			const string fullPath = "path/to/group";
			const string path = "path/to";
			const string name = "group";

			var group = _fixture.Create<Group>();

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Group);
			_pathResolver.Setup(x => x.DeconstructPath(fullPath)).Returns((name, path));
			_groupRepository.Setup(x => x.Get(name, path)).Returns(group).Verifiable();
			_groupRepository.Setup(x => x.DeleteGroup(name, path)).Verifiable();
			_secretsRepository.Setup(x => x.DeleteGroupSecrets(group.Id)).Verifiable();

			// act
			_target.DeleteSecrets(fullPath, Array.Empty<string>());

			// assert
			_groupRepository.Verify(x => x.DeleteGroup(name, path), Times.Once);
			_groupRepository.Verify(x => x.Get(name, path), Times.Once);
			_secretsRepository.Verify(x => x.DeleteGroupSecrets(group.Id), Times.Once);
		}

		[Test]
		public void DeleteNotFoundSecretTest()
		{
			// arrange
			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.None);

			// assert
			Assert.Throws<NotFoundException>(() => _target.DeleteSecrets("path/to/folder", Array.Empty<string>()));
		}

		[Test]
		public void DeleteSecretTest()
		{
			// arrange
			const string fullPath = "path/group/secret";
			const string secretName = "secret";
			const string groupPath = "path/group";
			const string groupName = "group";
			const string folderPath = "path";

			var group = _fixture.Create<Group>();

			_pathResolver.Setup(x => x.Resolve(fullPath)).Returns(PathType.Secret);
			_pathResolver.Setup(x => x.DeconstructPath(fullPath)).Returns((secretName, groupPath));
			_pathResolver.Setup(x => x.DeconstructPath(groupPath)).Returns((groupName, folderPath));
			_groupRepository.Setup(x => x.Get(groupName, folderPath)).Returns(group);
			_secretsRepository.Setup(x => x.DeleteGroupSecrets(group.Id, new[] { secretName })).Verifiable();

			// target
			_target.DeleteSecrets(fullPath, Array.Empty<string>());

			// assert
			_secretsRepository.Verify(x => x.DeleteGroupSecrets(group.Id, new[] { secretName }), Times.Once);
		}

		[Test]
		public void GetSecretsGroupTest()
		{
			// arrange
			var name = _fixture.Create<string>();
			var path = _fixture.Create<string>();
			var group = _fixture.Create<Group>();
			var mockSecret = _fixture.Create<Secret>();

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Group);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));

			_groupRepository.Setup(x => x.Get(name, path)).Returns(group);

			_secretsRepository.Setup(x => x.GetGroupSecrets(group.Id)).Returns(new List<Secret>
			{
				mockSecret
			});

			// act
			var result = _target.GetSecrets(path, Array.Empty<string>()).ToList();

			// assert
			Assert.AreEqual(result.Count, 1);
			Assert.AreEqual(result[0], mockSecret);
		}

		[Test]
		public void GetSecretsGroupWithNamesTest()
		{
			// arrange
			var group = _fixture.Create<Group>();

			var name = _fixture.Create<string>();
			var path = _fixture.Create<string>();

			var mockSecret = _fixture.Create<Secret>();
			var mockSecret2 = _fixture.Create<Secret>();

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Group);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));

			_groupRepository.Setup(x => x.Get(name, path)).Returns(group);

			_secretsRepository.Setup(x => x.GetGroupSecrets(group.Id)).Returns(new List<Secret>
			{
				mockSecret,
				mockSecret2
			});

			// act
			var result = _target.GetSecrets(path, new[] { mockSecret2.Name }).ToList();

			// assert
			Assert.AreEqual(result.Count, 1);
			Assert.AreEqual(result[0], mockSecret2);
		}

		[Test]
		public void GetSecretsGroupWithRandomNamesTest()
		{
			// arrange
			var group = _fixture.Create<Group>();

			var name = _fixture.Create<string>();
			var path = _fixture.Create<string>();

			var mockSecret = _fixture.Create<Secret>();
			var mockSecret2 = _fixture.Create<Secret>();

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Group);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));

			_groupRepository.Setup(x => x.Get(name, path)).Returns(group);

			_secretsRepository.Setup(x => x.GetGroupSecrets(group.Id)).Returns(new List<Secret>
			{
				mockSecret,
				mockSecret2
			});

			// act
			var result = _target.GetSecrets(path, new[] { _fixture.Create<string>() }).ToList();

			// assert
			Assert.AreEqual(result.Count, 0);
		}

		[Test]
		public void GetSecretsNoneTest()
		{
			// arrange
			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.None);

			// act

			// assert
			Assert.Throws<NotFoundException>(() => _target.GetSecrets("test", Array.Empty<string>()));
		}

		[Test]
		public void GetSecretsSecretNotFoundTest()
		{
			// arrange
			var group = _fixture.Create<Group>();

			var name = _fixture.Create<string>();
			var path = _fixture.Create<string>();

			var mockSecret = _fixture.Create<Secret>();

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Secret);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));

			_groupRepository.Setup(x => x.Get(name, path)).Returns(group);

			_secretsRepository.Setup(x => x.GetGroupSecrets(group.Id)).Returns(new List<Secret>
			{
				mockSecret
			});

			// assert
			Assert.Throws<NotFoundException>(() => _target.GetSecrets(_fixture.Create<string>(), Array.Empty<string>()));
		}

		[Test]
		public void GetSecretsSecretTest()
		{
			// arrange
			var name = _fixture.Create<string>();
			var path = _fixture.Create<string>();
			var group = _fixture.Create<Group>();
			var mockSecret = _fixture.Build<Secret>().With(x => x.Name, name).Create();

			_pathResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(PathType.Secret);
			_pathResolver.Setup(x => x.DeconstructPath(It.IsAny<string>())).Returns((name, path));

			_groupRepository.Setup(x => x.Get(name, path)).Returns(group);

			_secretsRepository.Setup(x => x.GetGroupSecrets(group.Id)).Returns(new List<Secret>
			{
				mockSecret
			});

			// act
			var result = _target.GetSecrets(path, Array.Empty<string>()).ToList();

			// assert
			Assert.AreEqual(result.Count, 1);
			Assert.AreEqual(result[0], mockSecret);
		}
	}
}