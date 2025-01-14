using Courses.Domain.Entitites;
using Courses.Features.Teacher.Create;
using Courses.Utils;

namespace Courses.Services.Interfaces;

public interface ITeacherService
{
    Task<Result<Teacher>> CreateAsync(TeacherCreateRequest classInput);
    Result<Teacher> GetbyId(string rfc);
}