using System.Collections.Generic;

using Microsoft.AspNetCore.Http;


namespace Keebox.SecretsService.Services
{
	public interface IFileConverter
	{
		Dictionary<string, string> Convert(Dictionary<string, IFormFile>? form);
	}
}