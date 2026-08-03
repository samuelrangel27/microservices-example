namespace Courses.Domain.Entitites;

public class Course
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Teacher? Teacher { get; set; }
    public Subject Subject { get; set; } = new();
    public ICollection<Student>? Students { get; set; }

    public Cycle Cycle { get; set; }
}