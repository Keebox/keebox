using System;


namespace Keebox.Common.DataAccess.Entities
{
	public sealed record Secret : Entity
	{
		public Guid GroupId { get; init; }

		public string Name { get; init; } = default!;

		public string Value { get; set; } = default!;

		public bool IsFile { get; init; }
	}
}