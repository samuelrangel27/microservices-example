using Courses.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.DbContexts.Configuration;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("uniqueidentifier").ValueGeneratedOnAdd();
        builder.OwnsMany(x => x.Students, xc =>
        {
            xc.ToJson();
            xc.Property(x => x.StudentKey).HasJsonPropertyName("StudentId");
            xc.Property(x => x.Name).HasColumnType("nvarchar(100)");
        });
        builder.OwnsOne(x => x.Subject, xc =>
        {
            xc.ToJson();
            xc.Property(x => x.SubjectKey).HasJsonPropertyName("SubjectId");
            xc.Property(x => x.Name).HasColumnType("nvarchar(100)");
        });
    }
}