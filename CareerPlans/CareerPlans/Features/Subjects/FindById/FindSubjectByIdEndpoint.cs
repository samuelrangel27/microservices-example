using CareerPlans.Services.Interfaces;
using FastEndpoints;
using IdempotentAPI.MinimalAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CareerPlans.Features.Subjects.FindById;

public class FindSubjectByIdEndpoint: Endpoint<FindSubjectByIdRequest,Results<Ok<FindSubjectByIdResponse>,NotFound>>
{
    private readonly ILogger<FindSubjectByIdEndpoint> logger;
    private readonly ISubjectService subjectService;

    public FindSubjectByIdEndpoint(ISubjectService subjectService,
        ILogger<FindSubjectByIdEndpoint> logger)
    {
        this.logger = logger;
        this.subjectService = subjectService;
    }
    public override void Configure()
    {
        Get("/api/subjects/{subjectId}");
        AllowAnonymous();
        //Options(x => x.AddEndpointFilter<IdempotentAPIEndpointFilter>());
    }
    public override async Task<Results<Ok<FindSubjectByIdResponse>,NotFound>> ExecuteAsync(FindSubjectByIdRequest req,CancellationToken ct)
    {
        logger.LogInformation("Getting subject for SubjectId: '{SubjectId}'",req.SubjectId);
        var s = await subjectService.GetById(req.SubjectId);
        logger.LogInformation("Subject for '{subjectId}': {@s}",req.SubjectId,s);
        if (s is null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(new FindSubjectByIdResponse
        {
            Subject = s
        });
    }
}