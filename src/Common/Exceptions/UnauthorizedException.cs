using System;


namespace Keebox.Common.Exceptions
{
	public class UnauthorizedException : Exception
	{
		public UnauthorizedException(string message) : base(message) { }
	}
}