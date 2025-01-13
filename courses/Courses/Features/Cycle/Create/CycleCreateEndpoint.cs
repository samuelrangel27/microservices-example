using Courses.Services.Interfaces;

namespace Courses.Features.Cycle.Create;

public class CycleCreateEndpoint : Endpoint<CycleCreateRequest, CycleCreateResponse>
{
    public ISchoolCycleService CycleService { get; set; }
    public override void Configure()
    {
        Post("/api/cycle");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CycleCreateRequest req, CancellationToken ct)
    {
        var r = await CycleService.add(req);
        r.EnsureSuccess();
        await SendAsync(new()
        {
            CycleId = r.Data.Id
        });
    }
}