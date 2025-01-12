﻿namespace Keebox.SecretsService
{
	public static class RouteMap
	{
		public const string Any = "{*route}";

		public const string Login = "login";

		public const string Role = "role";

		public const string Permission = "permission";

		public const string RoleAssigning = "assign";

		public static class Account
		{
			public const string Base = "account";

			public const string Assign = "assign";
		}

		public static class System
		{
			public const string Base = "system";

			public const string Config = Base + "/config";

			public const string Status = Base + "/status";

			public const string Start = Base + "/start";

			public const string Stop = Base + "/stop";
		}
	}
}