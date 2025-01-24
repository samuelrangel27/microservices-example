using CareerPlans.Entities;
using CareerPlans.Services.Interfaces;
using FastEndpoints;
using IdempotentAPI.MinimalAPI;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CareerPlans.Features.Subjects.Create;

public class CreateSubjectEndpoint(ISubjectService subjectService) : Endpoint<CreateSubjectRequest, Results<Ok<CreateSubjectResponse>,ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/subjects");
        AllowAnonymous();
        //Options(x => x.AddEndpointFilter<IdempotentAPIEndpointFilter>());
    }
    public override async Task<Results<Ok<CreateSubjectResponse>,ProblemDetails>> ExecuteAsync(CreateSubjectRequest req, CancellationToken ct)
    {
        var sid = await subjectService.AddSubjectAsync(req);
        return TypedResults.Ok(new CreateSubjectResponse
        {
            SubjectId = sid
        });
    }
}