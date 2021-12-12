using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;

using Keebox.Common.Managers;
using Keebox.Common.Types;
using Keebox.SecretsService.Exceptions;
using Keebox.SecretsService.Middlewares.Attributes;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSwag.Annotations;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Route(RouteMap.Any)]
	[Authenticate] [AuthorizeForGroup]
	[SuppressMessage("ReSharper", "RouteTemplates.MethodMissingRouteParameters")]
	[SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
	public class SecretsController : ControllerBase
	{
		public SecretsController(
			ISecretManager secretManager, IFileConverter fileConverter, IFormatterResolver formatterResolver, Configuration configuration,
			ILogger<SecretsController> logger
		)
		{
			_secretManager = secretManager;
			_fileConverter = fileConverter;
			_formatterResolver = formatterResolver;
			_configuration = configuration;
			_logger = logger;
		}

		[HttpPut]
		[OpenApiOperation("Save secrets",
			"If route leads to non-existent group it is created and secrets are saved."
			+ "If route leads to group secrets are created/replaced."
			+ "If route leads to secret it is updated.")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult AddSecrets([FromRoute] RequestPayload payload)
		{
			_logger.LogInformation($"Adding secrets {payload.Route}");

			if (payload.Route is null)
				throw new EmptyRouteException();

			if ((payload.Data is null || !payload.Data.Keys.Any()) && (payload.Files is null || !payload.Files.Keys.Any()))
				throw new SecretsNotProvidedException();

			_secretManager.AddSecrets(payload.Route, payload.Data!.ToDictionary(x => x.Key, x => x.Value.ToString()!),
				_fileConverter.Convert(payload.Files),
				ExtractSecretsFromRequest(payload));

			return NoContent();
		}

		[HttpGet]
		[OpenApiOperation("Get secrets",
			"If route leads to group secrets are returned."
			+ "If route leads to secret it is returned."
			+ "If route leads to file it is returned as stream.")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(string))]
		[SwaggerResponse(HttpStatusCode.OK, typeof(File))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult GetSecrets([FromRoute] RequestPayload payload)
		{
			_logger.LogInformation($"Getting secrets {payload.Route}");

			if (payload.Route is null) throw new EmptyRouteException();

			var secrets = _secretManager.GetSecrets(payload.Route, ExtractSecretsFromRequest(payload)).ToArray();

			if (secrets.Length == 1)
			{
				var secret = secrets.Single();

				if (!secret.IsFile)
					return Ok(secret.Value);

				var stream = new MemoryStream(_fileConverter.Decode(secret.Value));

				return File(stream, "application/octet-stream");
			}

			if (!payload.IncludeFiles)
				secrets = secrets.Where(x => !x.IsFile).ToArray();

			var format = payload.Format ?? _configuration.DefaultFormat;
			var formatter = _formatterResolver.Resolve(format);

			HttpContext.Response.ContentType = ResolveContentType(format);

			return Ok(formatter.Format(secrets));
		}

		[HttpDelete]
		[OpenApiOperation("Delete secrets", "If route leads to group it is deleted." + "If route leads to secret if is deleted.")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult DeleteSecrets([FromRoute] RequestPayload payload)
		{
			_logger.LogInformation($"Deleting secrets {payload.Route}");

			if (payload.Route is null)
			{
				throw new EmptyRouteException();
			}

			_secretManager.DeleteSecrets(payload.Route, ExtractSecretsFromRequest(payload));

			return NoContent();
		}

		private static string[] ExtractSecretsFromRequest(RequestPayload payload)
		{
			return payload.Secrets?.Split(SecretsSeparator) ?? Array.Empty<string>();
		}

		private static string ResolveContentType(FormatType? format)
		{
			return format switch
			{
				FormatType.Env  => "text/plain",
				FormatType.Json => "application/json",
				FormatType.Xml  => "text/plain",
				_               => "text/plain"
			};
		}

		private const char SecretsSeparator = ',';

		private readonly Configuration _configuration;
		private readonly IFileConverter _fileConverter;
		private readonly IFormatterResolver _formatterResolver;
		private readonly ILogger<SecretsController> _logger;

		private readonly ISecretManager _secretManager;
	}
}