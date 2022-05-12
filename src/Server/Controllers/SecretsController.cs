using Contracts;

using Microsoft.AspNetCore.Mvc;

using Services;


namespace Server.Controllers;

[ApiController]
[Route("{*route}")]
public class SecretsController : ControllerBase
{
	private readonly ISecretsService _secretsService;
	private readonly IPathResolver _pathResolver;

	public SecretsController(ISecretsService secretsService, IPathResolver pathResolver)
	{
		_secretsService = secretsService;
		_pathResolver = pathResolver;
	}

	[HttpPut]
	public void SaveSecrets([FromRoute] SecretsPayload payload)
	{
		var pathType = _pathResolver.Resolve(payload.Route);

		switch (pathType)
		{
			case PathType.None:
				_secretsService.CreateGroup(payload.Route, payload.Data ?? new Dictionary<string, string>());

				break;
			case PathType.Group:
				_secretsService.UpdateGroup(payload.Route, payload.Data ?? new Dictionary<string, string>());

				break;
			case PathType.Secret:
				throw new ArgumentException("Updating secret is not yet supported");
		}
	}

	[HttpGet]
	public object GetSecrets([FromRoute] string route)
	{
		var pathType = _pathResolver.Resolve(route);

		switch (pathType)
		{
			case PathType.None:
				throw new ArgumentException("Not found");
			case PathType.Group:
				var secrets = _secretsService.GetGroupSecrets(route);

				return secrets.ToDictionary(x => x.Name, x => x.Value);
			case PathType.Secret:
				var secret = _secretsService.GetSecret(route);

				return secret.Value!;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	[HttpDelete]
	public void DeleteSecrets([FromRoute] string route)
	{
		var pathType = _pathResolver.Resolve(route);

		switch (pathType)
		{
			case PathType.None:
				throw new ArgumentException("Not found");
			case PathType.Group:
				_secretsService.DeleteGroup(route);

				break;
			case PathType.Secret:
				_secretsService.DeleteSecret(route);

				break;
		}
	}
}