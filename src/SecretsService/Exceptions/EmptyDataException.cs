using System;


namespace Keebox.SecretsService.Exceptions
{
	public class EmptyDataException : Exception
	{
		public EmptyDataException(string message) : base(message) { }
	}
}