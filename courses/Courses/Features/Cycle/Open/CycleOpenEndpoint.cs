using Courses.Features.Cycle.Create;
using Courses.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Courses.Features.Cycle.Open;

public class CycleOpenEndpoint : Endpoint<CycleOpenRequest, Results<NoContent,ProblemDetails>>
{
    public ISchoolCycleService CycleService { get; set; }
    public override void Configure()
    {
        Put("/api/cycle/open");
        Description((x => x.Accepts<CycleOpenRequest>()));
        RequestBinder(new CycleOpenRequestBinder());
        AllowAnonymous();
    }

    public override async Task<Results<NoContent,ProblemDetails>> HandleAsync(CycleOpenRequest req, CancellationToken ct)
    {
        var r = await CycleService.OpenAsync();
        r.EnsureSuccess();
        return TypedResults.NoContent();
    }
}