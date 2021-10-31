using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Helpers;
using Keebox.Common.Types;

using Moq;

using NUnit.Framework;


namespace Keebox.Common.UnitTests.Helpers
{
	[TestFixture]
	public class PathResolverTests
	{
		[SetUp]
		public void Setup()
		{
			_repositoryContext = new Mock<IRepositoryContext>();
			_groupRepository = new Mock<IGroupRepository>();

			_repositoryContext.Setup(x => x.GetGroupRepository()).Returns(_groupRepository.Object);
			_target = new PathResolver(_repositoryContext.Object);
		}

		private Mock<IRepositoryContext> _repositoryContext;
		private Mock<IGroupRepository> _groupRepository;

		private IPathResolver _target;

		[Test]
		[TestCase("path/to/folder", "path/to", "folder")]
		[TestCase("path/to", "path", "to")]
		[TestCase("path", "", "path")]
		public void DeconstructTest(string fullPath, string expectedPath, string expectedName)
		{
			// act
			var (name, path) = _target.DeconstructPath(fullPath);

			// assert
			Assert.AreEqual(expectedName, name);
			Assert.AreEqual(expectedPath, path);
		}

		[Test]
		public void Resolve_NotExistingSingleFolderPathTest()
		{
			// arrange
			const string path = "path";

			_groupRepository.Setup(x => x.Exists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			// act
			var result = _target.Resolve(path);

			// assert
			Assert.AreEqual(PathType.None, result);

			_groupRepository.Verify(x => x.Exists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Resolve_PathToGroupTest()
		{
			// arrange
			const string fullPath = "path/test";
			const string name = "test";
			const string path = "path";

			_groupRepository.Setup(x => x.Exists(name, path)).Returns(true);

			// act
			var result = _target.Resolve(fullPath);

			// assert
			Assert.AreEqual(PathType.Group, result);

			_groupRepository.Verify(x => x.Exists(name, path), Times.Once);
		}

		[Test]
		public void Resolve_PathToSecretTest()
		{
			// arrange
			const string fullPath = "path/to/folder";
			const string name = "folder";
			const string path = "path/to";

			_groupRepository.Setup(x => x.Exists(name, path)).Returns(false);

			var (name2, abovePath) = _target.DeconstructPath(path);

			_groupRepository.Setup(x => x.Exists(name2, abovePath)).Returns(true);

			// act
			var result = _target.Resolve(fullPath);

			// assert
			Assert.AreEqual(PathType.Secret, result);

			_groupRepository.Verify(x => x.Exists(name, path), Times.Once);
			_groupRepository.Verify(x => x.Exists(name2, abovePath), Times.Once);
		}

		[Test]
		public void Resolve_RandomPathTest()
		{
			// arrange
			const string path = "not/existing/path";

			_groupRepository.Setup(x => x.Exists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			// act
			var result = _target.Resolve(path);

			// assert
			Assert.AreEqual(PathType.None, result);

			_groupRepository.Verify(x => x.Exists(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
		}

		[Test]
		[TestCase("path/to/folder", true)]
		[TestCase("path", true)]
		[TestCase("|||", false)]
		public void ValidatePathTest(string fullPath, bool expectedResult)
		{
			// act
			var result = _target.ValidatePath(fullPath);

			// assert
			Assert.AreEqual(expectedResult, result);
		}
	}
}