using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.SecretsService.Services.Formatters
{
	public class JsonSecretFormatter : ISecretFormatter
	{
		public object Format(IEnumerable<Secret> data)
		{
			return data.ToDictionary(x => x.Name, x => x.Value);
		}
	}
}