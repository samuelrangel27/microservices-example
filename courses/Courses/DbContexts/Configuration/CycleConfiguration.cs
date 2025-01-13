using Courses.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.DbContexts.Configuration;

public class CycleConfiguration : IEntityTypeConfiguration<Cycle>
{
    public void Configure(EntityTypeBuilder<Cycle> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Status).HasColumnType("tinyint").IsRequired();
        builder.Property(c => c.StartDate).HasColumnType("date").IsRequired();
        builder.Property(c => c.EndDate).HasColumnType("date").IsRequired();
    }
}