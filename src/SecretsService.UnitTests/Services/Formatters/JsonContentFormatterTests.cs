using System.Collections.Generic;
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

		[Test]
		public void FormatTest()
		{
			// arrange
			var data = _fixture.CreateMany<Secret>().ToList();
			var expectedData = data.ToDictionary(x => x.Name, x => x.Value);

			// act
			var result = _target.Format(data);

			// assert
			result.Should().BeEquivalentTo(expectedData);
		}

		private IFixture _fixture = new Fixture();

		private Mock<ISerializer> _serializer;

		private JsonSecretFormatter _target;
	}
}