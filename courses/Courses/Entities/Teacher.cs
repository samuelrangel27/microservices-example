namespace Courses.Domain.Entitites;

public class Teacher
{
    public string RFC { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProfessionalId { get; set; } = string.Empty;
    public string? Degree { get; set; }
    public ICollection<Course> Courses { get; set; }
}