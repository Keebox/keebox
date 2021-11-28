using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Keebox.Common.Managers;
using Keebox.Common.Types;
using Keebox.SecretsService.Exceptions;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.RequestFiltering;
using Keebox.SecretsService.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Keebox.SecretsService.Controllers
{
	[Authenticate]
	[ApiController]
	[Route(RouteMap.Any)]
	[SuppressMessage("ReSharper", "RouteTemplates.MethodMissingRouteParameters")]
	[SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
	public class SecretsController : ControllerBase
	{
		public SecretsController(
			ISecretManager             secretManager,
			IFileConverter             fileConverter,
			IFormatterResolver         formatterResolver,
			Configuration              configuration,
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
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetSecrets([FromRoute] RequestPayload payload)
		{
			_logger.LogInformation($"Getting secrets {payload.Route}");

			if (payload.Route is null)
				throw new EmptyRouteException();

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
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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