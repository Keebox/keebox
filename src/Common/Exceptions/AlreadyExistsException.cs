using System;


namespace Keebox.Common.Exceptions
{
	public class AlreadyExistsException : Exception
	{
		public AlreadyExistsException(string message) : base(message) { }
	}
}