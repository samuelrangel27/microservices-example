using Courses.DbContexts;
using Courses.Domain.Entitites;
using Courses.Features.Course.Create;
using Courses.Services.Interfaces;
using Courses.Utils;

namespace Courses.Services.Implementations;

public class CourseService(CoursesDbContext context, ITeacherService teacherService) : ICourseService
    {
        
        public async Task<Result<Course>> CreateAsync(CourseCreateRequest course)
        {
            // get open cycle
            var openCycle = context.Cycles.FirstOrDefault(x => x.Status == CycleStatus.Open);
            var errors = new List<string>();
            var newCourse = new Course
            {
                Description = course.Description
            };
                
            if(openCycle == null)
                errors.Add("No open cycle found");
            else
                newCourse.Cycle = openCycle;

            if(!string.IsNullOrEmpty(course.TeacherId))
            {
                var teacherResult = teacherService.GetbyId(course.TeacherId);
                if(teacherResult.IsSuccess)
                    newCourse.Teacher = teacherResult.Data;
                else
                    errors.Add(teacherResult.Message);
            }

            if(errors.Count > 0)
            {
                return Result<Course>.Fail("One or more errors occured when trying to add the course", errors);
            }

            context.Courses.Add(newCourse);
            await context.SaveChangesAsync();
            return Result<Course>.Ok(MsgConstants.SUCCESS, newCourse);
        } 
}