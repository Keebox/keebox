using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using AutoFixture;

using FluentAssertions;

using Keebox.SecretsService.Services;

using Microsoft.AspNetCore.Http;

using NUnit.Framework;


namespace Keebox.SecretsService.UnitTests.Services
{
	public class FileConverterTests
	{
		private IFixture _fixture;

		private IFileConverter _target;

		[SetUp]
		public void Setup()
		{
			_fixture = new Fixture();

			_target = new FileConverter();
		}

		[Test]
		public void ConvertForm()
		{
			// arrange
			const string fileKey = "file";

			using var mockStream = new MemoryStream();

			var content = _fixture.Create<string>();
			var file = Encoding.UTF8.GetBytes(content);
			mockStream.Write(file);

			var form = new Dictionary<string, IFormFile>
			{
				{ fileKey, new FormFile(mockStream, 0, mockStream.Length, fileKey, fileKey) }
			};

			// act
			var dictionary = _target.Convert(form);

			// assert
			dictionary.Keys.Count.Should().Be(1);
			dictionary.ContainsKey(fileKey).Should().BeTrue();

			Encoding.UTF8.GetString(Convert.FromBase64String(dictionary[fileKey]))
				.Should().Be(content);
		}

		[Test]
		public void ConvertEmptyForm()
		{
			// act
			var dictionary = _target.Convert(null);

			// assert
			dictionary.Keys.Count.Should().Be(0);
		}
	}
}