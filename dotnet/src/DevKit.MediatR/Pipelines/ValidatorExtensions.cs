using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;

namespace DevKit.MediatR.Pipelines;

public static class ValidatorExtensions
{
    public static async Task<(ValidationResult Result, string ValidatorName, TimeSpan ElapsedTime)>
        ValidateWithDiagnosticsAsync<T>(
            this IValidator<T> validator,
            T request,
            CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(validator);

        var start = Stopwatch.GetTimestamp();
        var result = await validator.ValidateAsync(request, cancellationToken);
        var elapsedTime = Stopwatch.GetElapsedTime(start);
        var validatorName = validator.GetType().Name;

        return (result, validatorName, elapsedTime);
    }
}
