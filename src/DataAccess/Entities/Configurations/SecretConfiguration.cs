using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccess.Entities.Configurations;

public class SecretConfiguration : IEntityTypeConfiguration<Secret>
{
	public void Configure(EntityTypeBuilder<Secret> builder)
	{
		builder.ToTable("secret");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id).ValueGeneratedOnAdd();
		builder.Property(x => x.CreatedAt).ValueGeneratedOnAdd();
	}
}