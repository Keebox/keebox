using System;


namespace Keebox.SecretsService.Exceptions
{
	public class UnsupportedFormatterException : Exception
	{
		public UnsupportedFormatterException(string formatter) : base($"{formatter} formatter is not supported") { }
	}
}