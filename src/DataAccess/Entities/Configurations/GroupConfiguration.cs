using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccess.Entities.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
	public void Configure(EntityTypeBuilder<Group> builder)
	{
		builder.ToTable("group");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id).ValueGeneratedOnAdd();
		builder.Property(x => x.CreatedAt).ValueGeneratedOnAdd();
		builder.HasMany(x => x.Secrets)
			.WithOne()
			.HasForeignKey(x => x.GroupId);
	}
}