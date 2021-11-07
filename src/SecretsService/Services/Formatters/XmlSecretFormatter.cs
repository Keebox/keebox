using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Helpers.Serialization;


namespace Keebox.SecretsService.Services.Formatters
{
	public class XmlSecretFormatter : ISecretFormatter
	{
		private readonly ISerializer? _serializer;

		public XmlSecretFormatter(ISerializer? serializer) =>
			_serializer = serializer;

		public object? Format(IEnumerable<Secret> data)
		{
			var outputData = data.Select(x => new Models.Secret
			{
				Name = x.Name,
				Value = x.Value
			});

			return _serializer?.Serialize(outputData.ToList());
		}
	}
}