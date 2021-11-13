using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.SecretsService.Services.Formatters
{
	public interface ISecretFormatter
	{
		object? Format(IEnumerable<Secret> data);
	}
}