using System;


namespace Keebox.Common.Helpers
{
	public interface IDateTimeProvider
	{
		DateTime UtcNow();
	}
}