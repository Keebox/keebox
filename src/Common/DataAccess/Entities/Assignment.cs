﻿using System;


namespace Keebox.Common.DataAccess.Entities
{
	public record Assignment : Entity
	{
		public Guid AccountId { get; init; }

		public Guid RoleId { get; init; }
	}
}