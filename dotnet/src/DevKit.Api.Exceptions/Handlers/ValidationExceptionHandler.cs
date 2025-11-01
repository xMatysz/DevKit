using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DevKit.Api.Exceptions.Handlers;

public class ValidationExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    private const int ValidationExceptionStatusCode = StatusCodes.Status400BadRequest;

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (exception is not ValidationException validationException)
        {
            return ValueTask.FromResult(false);
        }

        httpContext.Response.StatusCode = ValidationExceptionStatusCode;

        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                k => k.Key,
                v => v
                    .Select(x => x.ErrorMessage)
                    .ToArray());

        var problemDetails = new HttpValidationProblemDetails
        {
            Status = ValidationExceptionStatusCode,
            Title = "One or more validation errors occurred",
            Detail = validationException.Message,
            Errors = errors,
        };

        return problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = validationException,
                ProblemDetails = problemDetails,
            });
    }
}
