using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Helpers.Serialization;
using Keebox.SecretsService.Services.Formatters;

using Moq;

using NUnit.Framework;


namespace Keebox.SecretsService.UnitTests.Services.Formatters
{
	[TestFixture]
	public class JsonContentFormatterTests
	{
		[SetUp]
		public void Setup()
		{
			_fixture = new Fixture();

			_serializer = new Mock<ISerializer>(MockBehavior.Strict);

			_target = new JsonSecretFormatter(_serializer.Object);
		}

		private IFixture _fixture;

		private Mock<ISerializer> _serializer;

		private JsonSecretFormatter _target;

		[Test]
		public void FormatTest()
		{
			// arrange
			var data = _fixture.CreateMany<Secret>().ToList();
			var dictionary = data.ToDictionary(x => x.Name, x => x.Value);

			var serializedData = _fixture.Create<string>();

			_serializer.Setup(x => x.Serialize(dictionary)).Returns(serializedData);

			// act
			var result = _target.Format(data);

			// assert
			result.Should().Be(serializedData);

			_serializer.Verify(x => x.Serialize(dictionary), Times.Once);
		}
	}
}