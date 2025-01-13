namespace Courses.Domain.Entitites;

public class Cycle
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CycleStatus Status { get; set; }
    public ICollection<Course> Courses { get; set; }
}

public enum CycleStatus
{
    New,
    Open,
    Closed
}