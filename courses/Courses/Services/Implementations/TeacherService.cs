using Courses.DbContexts;
using Courses.Domain.Entitites;
using Courses.Features.Teacher.Create;
using Courses.Services.Interfaces;
using Courses.Utils;

namespace Courses.Services.Implementations;

public class TeacherService : ITeacherService
{
    private readonly CoursesDbContext dbContext;

    public TeacherService(CoursesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<Result<Teacher>> add(TeacherCreateRequest teacher)
    {
        var newTeacher = new Teacher{
            RFC = teacher.RFC,
            Name = teacher.Name,
        };
        await dbContext.Teachers.AddAsync(newTeacher);
        await dbContext.SaveChangesAsync();
        return Result<Teacher>.Ok(MsgConstants.SUCCESS, newTeacher);
    }

    public Result<Teacher> GetbyId(string rfc)
    {
        var teacher = dbContext.Teachers.FirstOrDefault(x => x.RFC == rfc);
        if(teacher != null)
            return Result<Teacher>.Ok(MsgConstants.SUCCESS,teacher);
        else
            return Result<Teacher>.Fail(string.Format(MsgConstants.NOTFOUND_WITH_ID,"Teacher",rfc));
    }
}