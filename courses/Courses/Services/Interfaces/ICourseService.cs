using Courses.Domain.Entitites;
using Courses.Features.Course.Create;
using Courses.Utils;

namespace Courses.Services.Interfaces;

public interface ICourseService
{
    Task<Result<Course>> add(CourseCreateRequest course);
    
}