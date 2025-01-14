using Courses.Features.Cycle.Create;
using Courses.Services.Interfaces;

namespace Courses.Features.Cycle.Open;

public class CycleOpenEndpoint : Endpoint<CycleOpenRequest, CycleOpenResponse>
{
    public ISchoolCycleService CycleService { get; set; }
    public override void Configure()
    {
        Put("/api/cycle/open");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CycleOpenRequest req, CancellationToken ct)
    {
        var r = await CycleService.OpenAsync();
        await SendAsync(new()
        {
        });
    }
}