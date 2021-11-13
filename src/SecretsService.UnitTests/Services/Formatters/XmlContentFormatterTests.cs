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
	public class XmlContentFormatterTests
	{
		[SetUp]
		public void Setup()
		{
			_serializer = new Mock<ISerializer>(MockBehavior.Strict);

			_target = new XmlSecretFormatter(_serializer.Object);
		}

		[Test]
		public void FormatTest()
		{
			// arrange
			var data = _fixture.CreateMany<Secret>().ToList();

			var projection = data.Select(x => new Keebox.SecretsService.Models.Secret
			{
				Name = x.Name,
				Value = x.Value
			});

			var serializedData = _fixture.Create<string>();

			_serializer.Setup(x => x.Serialize(projection)).Returns(serializedData);

			// act
			var result = _target.Format(data);

			// assert
			result.Should().Be(serializedData);

			_serializer.Verify(x => x.Serialize(projection), Times.Once);
		}

		private IFixture _fixture = new Fixture();

		private Mock<ISerializer> _serializer;

		private XmlSecretFormatter _target;
	}
}