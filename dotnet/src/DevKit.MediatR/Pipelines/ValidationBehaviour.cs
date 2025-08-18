using ErrorOr;
using FluentValidation;
using MediatR;

namespace DevKit.MediatR.Pipelines;

public class ValidationBehaviour<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var validationResults =
            await Task.WhenAll(_validators.Select(v => v.ValidateAsync(request, cancellationToken)));

        if (validationResults.All(x => x.IsValid))
        {
            return await next(cancellationToken);
        }

        if (typeof(TResult).IsAssignableFrom(typeof(IErrorOr)))
        {
            var errors = validationResults
                .SelectMany(y => y.Errors
                    .ConvertAll(error => Error.Validation(
                        code: error.PropertyName,
                        description: error.ErrorMessage)));
            return (dynamic)errors;
        }
        else
        {
            var errors = validationResults.SelectMany(x => x.Errors);
            throw new ValidationException(errors);
        }
    }
}
