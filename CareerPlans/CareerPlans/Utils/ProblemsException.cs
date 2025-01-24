using Microsoft.AspNetCore.Diagnostics;

namespace CareerPlans.Utils;

[Serializable]
public class ProblemsException: Exception
{
    public string Msg { get; set; }
    public IEnumerable<string> Errors { get; set; }

    public ProblemsException(string msg, IEnumerable<string> errors)
    {
        Msg = msg;
        Errors = errors;
    }
}

public class ProblemsExceptionHandler(IProblemDetailsService problemDetailsService) 
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProblemsException problemsException) return true;
        var det = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = problemsException.Msg,
            Type = "Bad Request",
            Extensions = new Dictionary<string, object?>()
            {
                {"errors", problemsException.Errors}
            }
        };
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = det
        });
    }
}