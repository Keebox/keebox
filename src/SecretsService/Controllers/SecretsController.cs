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
	[Authenticate]
	[ApiController]
	[Route("{*route}")]
	public class SecretsController : ControllerBase
	{
		public SecretsController(
			ISecretsManager            secretsManager,
			IFileConverter             fileConverter,
			IFormatterResolver         formatterResolver,
			Configuration              configuration,
			ILogger<SecretsController> logger
		)
		{
			_secretsManager = secretsManager;
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

			_secretsManager.AddSecrets(payload.Route ?? string.Empty, payload.Data,
				_fileConverter.Convert(payload.Files), secretNames);
		}

		[HttpGet]
		public IActionResult GetSecrets([FromRoute] RequestPayload payload)
		{
			_logger.LogDebug($"Getting secrets {payload.Route}");
			var secretNames = payload.Secrets?.Split(',') ?? Array.Empty<string>();

			var secrets = _secretsManager.GetSecrets(payload.Route ?? string.Empty, secretNames);

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

			_secretsManager.DeleteSecrets(payload.Route ?? string.Empty, secretNames);
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

		private readonly Configuration _configuration;
		private readonly IFileConverter _fileConverter;
		private readonly IFormatterResolver _formatterResolver;
		private readonly ILogger<SecretsController> _logger;

		private readonly ISecretsManager _secretsManager;
	}
}