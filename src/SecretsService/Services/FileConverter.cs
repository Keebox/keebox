using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Http;


namespace Keebox.SecretsService.Services
{
	public class FileConverter : IFileConverter
	{
		public Dictionary<string, string> Convert(Dictionary<string, IFormFile>? form)
		{
			if (form is null) return new Dictionary<string, string>();

			return form.ToDictionary(x => x.Key, x =>
			{
				var ms = new MemoryStream();
				x.Value.OpenReadStream().CopyTo(ms);

				return System.Convert.ToBase64String(ms.ToArray());
			});
		}

		public byte[] Decode(string encoded)
		{
			return System.Convert.FromBase64String(encoded);
		}
	}
}