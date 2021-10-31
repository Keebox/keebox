using System;


namespace Keebox.Common.DataAccess.Entities
{
	public record Secret : Entity
	{
		public Guid GroupId { get; init; }

		public string Name { get; init; }

		public string Value { get; set; }

		public bool IsFile { get; init; }
	}
}