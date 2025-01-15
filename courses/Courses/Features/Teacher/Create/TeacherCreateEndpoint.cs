using Courses.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Courses.Features.Teacher.Create;

public class TeacherCreateEndpoint : Endpoint<TeacherCreateRequest, Results<Ok, ProblemDetails>>
{
    public TeacherCreateEndpoint(ITeacherService teacherService)
    {
        this.TeacherService = teacherService;

    }
    public ITeacherService TeacherService { get; set; }
    public override void Configure()
    {
        Post("/api/teacher");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, ProblemDetails>> HandleAsync(TeacherCreateRequest req, CancellationToken ct)
    {
        var r = await TeacherService.CreateAsync(req);
        r.EnsureSuccess();
        return TypedResults.Ok();
    }
}