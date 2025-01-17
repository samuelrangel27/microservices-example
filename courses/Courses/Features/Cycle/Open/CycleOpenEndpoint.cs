using Courses.Features.Cycle.Create;
using Courses.Services.Interfaces;
using IdempotentAPI.MinimalAPI;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Courses.Features.Cycle.Open;

public class CycleOpenEndpoint : EndpointWithoutRequest<Results<NoContent, ProblemDetails>>
{
    public CycleOpenEndpoint(ISchoolCycleService cycleService)
    {
        this.CycleService = cycleService;
    }
    public ISchoolCycleService CycleService { get; set; }
    public override void Configure()
    {
        Put("/api/cycle/open");
        Description((x => x.Accepts<CycleOpenRequest>()));
        AllowAnonymous();
        Options(x => x.AddEndpointFilter<IdempotentAPIEndpointFilter>());
    }

    public override async Task<Results<NoContent, ProblemDetails>> HandleAsync(CancellationToken ct)
    {
        var r = await CycleService.OpenAsync();
        r.EnsureSuccess();
        return TypedResults.NoContent();
    }
}