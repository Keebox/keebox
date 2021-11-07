using Keebox.Common.Types;
using Keebox.SecretsService.Services.Formatters;


namespace Keebox.SecretsService.Services
{
	public interface IFormatterResolver
	{
		ISecretFormatter Resolve(FormatType? type);
	}
}