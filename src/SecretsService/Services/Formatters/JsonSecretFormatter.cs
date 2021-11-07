using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Helpers.Serialization;


namespace Keebox.SecretsService.Services.Formatters
{
	public class JsonSecretFormatter : ISecretFormatter
	{
		public JsonSecretFormatter(ISerializer? serializer)
		{
			_serializer = serializer;
		}

		public object? Format(IEnumerable<Secret> data)
		{
			return data.ToDictionary(x => x.Name, x => x.Value);
		}

		private readonly ISerializer? _serializer;
	}
}