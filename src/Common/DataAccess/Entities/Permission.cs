using System;


namespace Keebox.Common.DataAccess.Entities
{
	public record Permission : Entity
	{
		public Guid RoleId { get; set; }

		public Guid GroupId { get; set; }

		public bool IsReadOnly { get; set; }
	}
}