using System;


namespace Keebox.SecretsService.Exceptions
{
	public class UnsupportedTypeException : Exception
	{
		public UnsupportedTypeException(string message) : base(message) { }
	}
}