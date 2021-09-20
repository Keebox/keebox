﻿using FluentAssertions;

using NUnit.Framework;

using Ralfred.Common.Helpers.Serialization;


namespace SecretsService.UnitTests.Helpers
{
	[TestFixture]
	public class YamlSerializerTests
	{
		[SetUp]
		public void Setup()
		{
			_target = new YamlSerializer();
		}

		[Test]
		public void SerializeTest()
		{
			// arrange
			var @object = new { Field = "value" };

			// act
			var result = _target.Serialize(@object);

			// assert
			result.Should().NotBeNull();
			result!.Trim().Should().BeEquivalentTo("field: value");
		}

		[Test]
		public void DeserializeTest()
		{
			// arrange
			var serializedObject = "first: value1\r\nsecond: value2";

			var expected = new TempEntity
			{
				First = "value1",
				Second = "value2"
			};

			// act
			var result = _target.Deserialize<TempEntity>(serializedObject);

			// assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(expected);
		}

		private YamlSerializer _target;
	}

	internal class TempEntity
	{
		public string First { get; init; }

		public string Second { get; init; }
	}
}