using System;
using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.SecretsService.Services.Formatters
{
	public class KeyValueSecretFormatter : ISecretFormatter
	{
		public string Format(IEnumerable<Secret> data)
		{
			return string.Join(Environment.NewLine, data.Select(x => $"{x.Name}={x.Value}"));
		}
	}
}