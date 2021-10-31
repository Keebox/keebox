using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Formatters;


namespace Keebox.SecretsService.RequestFiltering
{
	public class BypassFormDataInputFormatter : IInputFormatter
	{
		public bool CanRead(InputFormatterContext context)
		{
			return true;
		}

		public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
		{
			return InputFormatterResult.SuccessAsync(null!);
		}
	}
}