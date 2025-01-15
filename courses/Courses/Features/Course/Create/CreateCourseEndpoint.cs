using Courses.Features.Cycle.Create;
using Courses.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Courses.Features.Course.Create;

public class CreateCourseEndpoint : Endpoint<CourseCreateRequest, Results<Ok<CourseCreateResponse>,ProblemDetails>>
{
    public ICourseService CourseService { get; set; }
    public override void Configure()
    {
        Post("/api/course");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<CourseCreateResponse>, ProblemDetails>> HandleAsync(CourseCreateRequest req, CancellationToken ct)
    {
        var r = await CourseService.CreateAsync(req);
        r.EnsureSuccess();
        return TypedResults.Ok(new CourseCreateResponse
        {
            CourseId = r.Data.Id
        });
    }
}