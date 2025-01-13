using Courses.Services.Interfaces;

namespace Courses.Features.Teacher.Create;

public class TeacherCreateEndpoint : Endpoint<TeacherCreateRequest, TeacherCreateResponse>
{
    public ITeacherService TeacherService { get; set; }
    public override void Configure()
    {
        Post("/api/teacher");
        AllowAnonymous();
    }

    public override async Task HandleAsync(TeacherCreateRequest req, CancellationToken ct)
    {
        var r = await TeacherService.add(req);
        await SendAsync(new()
        {
        });
    }
}