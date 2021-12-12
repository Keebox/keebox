using System;


namespace Keebox.Common.Helpers
{
	public class DateTimeProvider : IDateTimeProvider
	{
		public DateTime UtcNow()
		{
			return DateTime.UtcNow;
		}
	}
}