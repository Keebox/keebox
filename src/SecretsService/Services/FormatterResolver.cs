using System;

using Keebox.Common.DependencyInjection;
using Keebox.Common.Types;
using Keebox.SecretsService.Exceptions;
using Keebox.SecretsService.Services.Formatters;


namespace Keebox.SecretsService.Services
{
	public class FormatterResolver : IFormatterResolver
	{
		private readonly StorageResolvingExtensions.SerializerResolver _serializerResolver;

		public FormatterResolver(StorageResolvingExtensions.SerializerResolver serializerResolver) =>
			_serializerResolver = serializerResolver;

		public ISecretFormatter Resolve(FormatType? type)
		{
			var serializer = _serializerResolver(type);

			return type switch
			{
				FormatType.Env  => new KeyValueSecretFormatter(),
				FormatType.Json => new JsonSecretFormatter(serializer),
				FormatType.Xml  => new XmlSecretFormatter(serializer),

				_ => throw new UnsupportedFormatterException(type.ToString()
															 ?? throw new ArgumentOutOfRangeException(nameof(type), type, null))
			};
		}
	}
}