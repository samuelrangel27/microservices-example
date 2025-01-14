using Courses.Features.Cycle.Create;
using Courses.Services.Interfaces;

namespace Courses.Features.Course.Create;

public class CreateCourseEndpoint : Endpoint<CourseCreateRequest, CourseCreateResponse>
{
    public ICourseService CourseService { get; set; }
    public override void Configure()
    {
        Post("/api/course");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CourseCreateRequest req, CancellationToken ct)
    {
        var r = await CourseService.CreateAsync(req);
        await SendAsync(new()
        {
            CourseId = r.Data.Id
        });
    }
}