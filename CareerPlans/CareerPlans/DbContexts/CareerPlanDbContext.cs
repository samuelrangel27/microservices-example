using CareerPlans.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerPlans.DbContexts;

public class CareerPlanDbContext : DbContext
{
    public CareerPlanDbContext(){}
    public CareerPlanDbContext(DbContextOptions<CareerPlanDbContext> options) : base(options) {}

    public DbSet<Career> Careers { get; set; }
    public DbSet<Subject?> Subjects { get; set; }
    public DbSet<CareerPlans.Entities.CareerPlan> CareerPlans { get; set; }
}