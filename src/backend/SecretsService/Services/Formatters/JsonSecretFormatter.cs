using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Helpers.Serialization;


namespace Keebox.SecretsService.Services.Formatters
{
	public class JsonSecretFormatter : ISecretFormatter
	{
		private readonly ISerializer? _serializer;

		public JsonSecretFormatter(ISerializer? serializer) =>
			_serializer = serializer;

		public string? Format(IEnumerable<Secret> data)
		{
			var dictionary = data.ToDictionary(x => x.Name, x => x.Value);

			return _serializer?.Serialize(dictionary);
		}
	}
}