using System;
using System.Collections.Generic;
using System.Linq;

using Keebox.Common.Types;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Models
{
	[Serializable]
	public record RequestPayload(string? Route)
	{
		[FromBody] public Dictionary<string, object>? Body { get; init; }

		[FromForm] public IFormCollection? FormData { get; init; }

		[FromQuery] public string? Secrets { get; init; }

		[FromQuery] public FormatType? Format { get; set; }

		[FromQuery] public bool IncludeFiles { get; set; }

		public Dictionary<string, object>? Data
		{
			get => Body ?? FormData?.ToDictionary(x => x.Key, x => (object) x.Value.ToString());
		}

		public Dictionary<string, IFormFile>? Files
		{
			get => FormData?.Files.ToDictionary(x => x.Name, x => x);
		}
	}
}