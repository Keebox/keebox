using System;


namespace Keebox.Common.DataAccess.Entities
{
	public record Account : Entity
	{
		public string? Name { get; init; }

		public string? TokenHash { get; set; }

		public string? CertificateThumbprint { get; init; }

		public Guid[] RoleIds { get; init; }
	}
}