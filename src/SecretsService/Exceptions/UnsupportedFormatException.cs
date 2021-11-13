using System;


namespace Keebox.SecretsService.Exceptions
{
	public class UnsupportedFormatException : Exception
	{
		public UnsupportedFormatException(string formatter) : base($"{formatter} format is not supported.") { }
	}
}