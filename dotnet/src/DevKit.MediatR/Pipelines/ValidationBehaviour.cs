using System.Diagnostics;
using System.Text.Json;
using DevKit.Base;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace DevKit.MediatR.Pipelines;

public sealed class ValidationBehaviour<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private const string BaseActivityName = nameof(ValidationBehaviour<TRequest, TResult>);
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        using var validationActivity = ApplicationDiagnostics.StartActivity(BaseActivityName);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        if (!_validators.Any())
        {
            validationActivity?.FinishActivity(new ActivityEvent("No validators"));
            return await next(cancellationToken);
        }

        var validationResults = await Task.WhenAll(_validators.Select(async v => await v.ValidateWithDiagnosticsAsync(request, cancellationToken)));

        var validatorTags = validationResults
            .Select(res => new KeyValuePair<string, object?>(
                res.ValidatorName,
                JsonSerializer.Serialize(
                    new
                    {
                        res.ElapsedTime,
                        res.Result.IsValid,
                        Errors = res.Result.Errors.Select(x => x.ErrorMessage),
                    },
                    JsonSerializerOptions.Default)))
            .ToArray();

        if (validationResults.All(x => x.Result.IsValid))
        {
            validationActivity?.FinishActivity(new ActivityEvent("validation finished", tags: [..validatorTags]));
            return await next(cancellationToken);
        }

        if (typeof(TResult).IsAssignableFrom(typeof(IErrorOr)))
        {
            var errors = validationResults
                .SelectMany(y => y.Result.Errors
                    .ConvertAll(error => Error.Validation(
                        code: error.PropertyName,
                        description: error.ErrorMessage)));

            validationActivity?.SetFailure(tags: validatorTags);

            return (dynamic)errors;
        }
        else
        {
            var errors = validationResults
                .SelectMany(x => x.Result.Errors)
                .ToArray();

            const string errorMessage = "Your request parameters did not validate";

            var exception = new ValidationException(errorMessage, errors, appendDefaultMessage: false);
            validationActivity?.SetFailure(exception, validatorTags);
            throw exception;
        }
    }
}
