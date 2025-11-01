using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DevKit.Api.Exceptions.Handlers;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    private const int GlobalExceptionHandlerStatusCode = StatusCodes.Status500InternalServerError;

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = GlobalExceptionHandlerStatusCode;

        var problemDetails = new HttpValidationProblemDetails
        {
            Status = GlobalExceptionHandlerStatusCode,
            Title = "Internal Server Error",
        };

        return problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails,
            });
    }
}
