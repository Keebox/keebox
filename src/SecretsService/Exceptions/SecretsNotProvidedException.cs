using System;


namespace Keebox.SecretsService.Exceptions
{
	public class SecretsNotProvidedException : Exception
	{
		public SecretsNotProvidedException() : base($"Secrets are not provided.") { }
	}
}