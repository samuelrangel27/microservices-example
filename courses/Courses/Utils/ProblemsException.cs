using Microsoft.AspNetCore.Diagnostics;

namespace Courses.Utils;

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
        ProblemsException ex = (ProblemsException)exception;
        var det = new ProblemDetails
        {
            Status = 400,
            
            Errors = problemsException.Errors.Select(x => new ProblemDetails.Error()),
        };
    }
}