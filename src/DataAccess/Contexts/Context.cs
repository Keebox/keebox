using System.Reflection;

using Microsoft.EntityFrameworkCore;


namespace DataAccess.Contexts;

internal class Context : DbContext
{
	public Context(DbContextOptions options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}