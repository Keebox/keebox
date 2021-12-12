namespace Keebox.Common.DataAccess.Entities
{
	public sealed record Account : Entity
	{
		public string Name { get; init; } = default!;

		public string? TokenHash { get; set; }

		public string? CertificateThumbprint { get; init; }

		/*
		 *
		 * TODO: Add Creation and Modification Timestamps for auditing.
		 */
	}
}