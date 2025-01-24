namespace CareerPlans.Entities;

public class CareerPlan
{
    public Guid Id { get; set; }
    public Career Career { get; set; }
    public IList<Subject> Subjects { get; set; }
    public short Year { get; set; }
    public CareerPlanStatus Status { get; set; }
}

public enum CareerPlanStatus
{
    New,
    Active,
    Completed
}