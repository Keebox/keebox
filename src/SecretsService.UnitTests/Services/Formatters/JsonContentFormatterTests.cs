using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Entities;
using Keebox.SecretsService.Services.Formatters;

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

			_target = new JsonSecretFormatter();
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

		private JsonSecretFormatter _target;
	}
}