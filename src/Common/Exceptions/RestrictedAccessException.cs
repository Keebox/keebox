using System;


namespace Keebox.Common.Exceptions
{
	public class RestrictedAccessException : Exception
	{
		public RestrictedAccessException(string message) : base(message) { }
	}
}