namespace Courses.Features.Course.Create;

public class CourseCreateRequest
{
    public string Description { get; set; }
    public string? TeacherId { get; set; }
    public int SubjectId { get; set; }
    public IList<int>? StudentIds { get; set; }
}