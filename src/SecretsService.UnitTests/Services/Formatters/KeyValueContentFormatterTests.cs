using System;
using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DataAccess.Entities;
using Keebox.SecretsService.Services.Formatters;

using NUnit.Framework;


namespace Keebox.SecretsService.UnitTests.Services.Formatters
{
	[TestFixture]
	public class KeyValueContentFormatterTests
	{
		[SetUp]
		public void Setup()
		{
			_target = new KeyValueSecretFormatter();
		}

		[Test]
		public void FormatTest()
		{
			// arrange
			var data = _fixture.CreateMany<Secret>().ToList();
			var expected = string.Join(Environment.NewLine, data.Select(x => $"{x.Name}={x.Value}"));

			// act
			var result = _target.Format(data);

			// assert
			result.Should().Be(expected);
		}

		private IFixture _fixture = new Fixture();

		private KeyValueSecretFormatter _target;
	}
}