using System;
using System.Linq;

using Keebox.Common.Managers;
using Keebox.Common.Types;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.RequestFiltering;
using Keebox.SecretsService.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Authenticate]
	[Route("{*route}")]
	public class SecretsController : ControllerBase
	{
		private readonly Configuration _configuration;
		private readonly IFileConverter _fileConverter;
		private readonly IFormatterResolver _formatterResolver;
		private readonly ILogger<SecretsController> _logger;

		private readonly ISecretManager _secretManager;

		public SecretsController(
			ISecretManager            secretManager,
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
		public void AddSecrets([FromRoute] RequestPayload payload)
		{
			_logger.LogDebug($"Adding secrets {payload.Route}");
			var secretNames = payload.Secrets?.Split(',') ?? Array.Empty<string>();

			if (payload.Data is null || !payload.Data.Keys.Any())
				throw new Exception("Secrets are not provided");

			_secretManager.AddSecrets(payload.Route ?? string.Empty, payload.Data,
				_fileConverter.Convert(payload.Files), secretNames);
		}

		[HttpGet]
		public IActionResult GetSecrets([FromRoute] RequestPayload payload)
		{
			_logger.LogDebug($"Getting secrets {payload.Route}");
			var secretNames = payload.Secrets?.Split(',') ?? Array.Empty<string>();

			var secrets = _secretManager.GetSecrets(payload.Route ?? string.Empty, secretNames);

			if (!payload.IncludeFiles)
				secrets = secrets.Where(x => !x.IsFile);

			var format = payload.Format ?? _configuration.DefaultFormat;

			var formatter = _formatterResolver.Resolve(format);
			HttpContext.Response.ContentType = ResolveContentType(format);

			return Ok(formatter.Format(secrets));
		}

		[HttpDelete]
		public void DeleteSecrets([FromRoute] RequestPayload payload)
		{
			_logger.LogDebug($"Deleting secrets {payload.Route}");
			var secretNames = payload.Secrets?.Split(',') ?? Array.Empty<string>();

			_secretManager.DeleteSecrets(payload.Route ?? string.Empty, secretNames);
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
	}
}