using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.Common.DependencyInjection;
using Keebox.Common.Helpers.Serialization;
using Keebox.Common.Types;
using Keebox.SecretsService.Exceptions;
using Keebox.SecretsService.Services;
using Keebox.SecretsService.Services.Formatters;

using Moq;

using NUnit.Framework;


namespace Keebox.SecretsService.UnitTests.Services
{
	[TestFixture]
	public class FormatterResolverTests
	{
		[SetUp]
		public void Setup()
		{
			_fixture = new Fixture();

			_serializerResolver = new Mock<StorageResolvingExtensions.SerializerResolver>();

			_target = new FormatterResolver(_serializerResolver.Object);
		}

		private IFixture _fixture;

		private Mock<ISerializer> _serializer;
		private Mock<StorageResolvingExtensions.SerializerResolver> _serializerResolver;

		private FormatterResolver _target;

		[Test]
		public void Resolve_KeyValueTest()
		{
			// arrange
			_serializer = new Mock<ISerializer>();

			var type = _fixture.Create<Generator<FormatType>>().FirstOrDefault(x => x == FormatType.Env);

			_serializerResolver.Setup(x => x(type)).Returns(_serializer.Object);

			// act
			var result = _target.Resolve(type);

			// assert
			result.GetType().Should().Be(typeof(KeyValueSecretFormatter));
		}

		[Test]
		public void Resolve_UndefinedTypeTest()
		{
			// arrange
			_serializer = new Mock<ISerializer>();

			var type = _fixture.Create<Generator<FormatType>>().FirstOrDefault(x => !new[]
			{
				FormatType.Json,
				FormatType.Xml,
				FormatType.Env
			}.Contains(x));

			_serializerResolver.Setup(x => x(type)).Returns(_serializer.Object);

			// act
			Assert.Throws<UnsupportedFormatException>(() => _target.Resolve(type));
		}

		[Test]
		public void ResolveTest()
		{
			// arrange
			_serializer = new Mock<ISerializer>();

			var type = _fixture.Create<Generator<FormatType>>().FirstOrDefault(x => new[]
			{
				FormatType.Json,
				FormatType.Xml
			}.Contains(x));

			_serializerResolver.Setup(x => x(type)).Returns(_serializer.Object);

			// act
			var result = _target.Resolve(type);

			// assert
			new[] { typeof(JsonSecretFormatter), typeof(XmlSecretFormatter) }.Contains(result.GetType()).Should().BeTrue();
		}
	}
}