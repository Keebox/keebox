using System;


namespace Keebox.Common.DataAccess.Entities
{
	public record Role : Entity
	{
		public Role(string name)
		{
			Id = Guid.NewGuid();
			Name = name;
		}

		public string Name { get; set; }
	}
}