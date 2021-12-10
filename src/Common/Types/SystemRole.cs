namespace Keebox.Common.Types
{
	public enum SystemRole
	{
		Admin = 0
	}

	public static class FormattedSystemRole
	{
		public static string Admin = nameof(SystemRole.Admin).ToLower();
	}
}