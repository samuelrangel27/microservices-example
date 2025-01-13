using Courses.Domain.Entitites;
using Courses.DbContexts.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Courses.DbContexts;

public class CoursesDbContext: DbContext
{
    public CoursesDbContext()
    {
    }

    public CoursesDbContext(DbContextOptions<CoursesDbContext> options) : base(options)
    {
    }
    
    public DbSet<Cycle> Cycles { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Teacher> Teachers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseConfiguration).Assembly);
    }
}