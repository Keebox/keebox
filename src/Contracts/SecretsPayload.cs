using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Contracts;

[Serializable]
public class SecretsPayload
{
	public string Route { get; init; }

	[FromBody] public Dictionary<string, object>? Body { get; set; }

	[FromForm] public IFormCollection? FormData { get; set; }

	public Dictionary<string, string>? Data
	{
		get =>
			Body?.ToDictionary(x => x.Key, x => x.Value.ToString())
			?? FormData?.ToDictionary(x => x.Key, x => x.Value.ToString());
	}

	public Dictionary<string, byte[]>? Files
	{
		get =>
			FormData?.Files.ToDictionary(x => x.Name, x =>
			{
				var ms = new MemoryStream();
				x.OpenReadStream().CopyTo(ms);

				return ms.ToArray();
			});
	}
}