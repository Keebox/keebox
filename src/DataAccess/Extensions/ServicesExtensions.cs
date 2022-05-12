using System.Reflection;

using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repository;

using FluentMigrator.Runner;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DataAccess.Extensions;

public static class ServicesExtensions
{
	public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<Context>(builder =>
		{
			builder.UseNpgsql(configuration.GetConnectionString("Postgres")).UseSnakeCaseNamingConvention();
		});

		services.AddTransient<IRepository<Group>, Repository<Group>>();
		services.AddTransient<IRepository<Secret>, Repository<Secret>>();
		services.AddFluentMigratorCore()
			.ConfigureRunner(builder =>
			{
				builder.AddPostgres()
					.WithGlobalConnectionString(configuration.GetConnectionString("Postgres"))
					.ScanIn(Assembly.GetExecutingAssembly()).For.Migrations();
			});

		return services;
	}

	public static IApplicationBuilder ApplyDatabaseMigrations(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();
		scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();

		return app;
	}
}