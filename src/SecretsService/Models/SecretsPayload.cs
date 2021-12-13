using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

using Keebox.Common.Types;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using NSwag.Annotations;


namespace Keebox.SecretsService.Models
{
	[Serializable]
	[Description("Route can lead to secret or secrets group")]
	public record SecretsPayload
	{
		[JsonPropertyName("route")] public string Route { get; init; } = default!;

		[Description("Secrets can be provided via body")]
		[FromBody] public Dictionary<string, object>? Body { get; init; }

		[Description("Secrets can be provided via form")]
		[FromForm] public IFormCollection? FormData { get; init; }

		[Description("Actions will be performed only on specified secrets")]
		[FromQuery] public string? Secrets { get; init; }

		[Description("Specifies the format in which the secrets will be returned")]
		[FromQuery] public FormatType? Format { get; set; }

		[Description("Whether to include files in base64 when group is requested")]
		[FromQuery] public bool IncludeFiles { get; set; }

		[OpenApiIgnore]
		public Dictionary<string, object>? Data
		{
			get => Body ?? FormData?.ToDictionary(x => x.Key, x => (object)x.Value.ToString());
		}

		[OpenApiIgnore]
		public Dictionary<string, IFormFile>? Files
		{
			get => FormData?.Files.ToDictionary(x => x.Name, x => x);
		}
	}
}