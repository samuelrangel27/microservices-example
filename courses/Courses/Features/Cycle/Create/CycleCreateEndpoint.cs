using Courses.Services.Interfaces;
using Courses.Utils;
using IdempotentAPI.MinimalAPI;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Courses.Features.Cycle.Create;

public class CycleCreateEndpoint : Endpoint<CycleCreateRequest, Results<Ok<CycleCreateResponse>,ProblemDetails>>
{
    public ISchoolCycleService CycleService { get; }

    public CycleCreateEndpoint(ISchoolCycleService cycleService)
    {
        this.CycleService = cycleService;
    }
    public override void Configure()
    {
        Post("/api/cycle");
        AllowAnonymous();
        Options(x => x.AddEndpointFilter<IdempotentAPIEndpointFilter>());
    }

    public override async Task<Results<Ok<CycleCreateResponse>,ProblemDetails>> HandleAsync(CycleCreateRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Cycle create operation started: {@req}", req);
        var r = await CycleService.CreateAsync(req);
        r.EnsureSuccess();
        return TypedResults.Ok(new CycleCreateResponse
        {
            CycleId = r.Data.Id
        });
    }
}