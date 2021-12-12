using System;


namespace Keebox.Common.DataAccess.Entities
{
	public sealed record Permission : Entity
	{
		public Guid RoleId { get; init; }

		public Guid GroupId { get; init; }

		public bool IsReadOnly { get; init; }
	}
}