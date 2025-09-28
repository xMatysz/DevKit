using DevKit.MediatR.Pipelines;
using MediatR;

namespace DevKit.MediatR.Cqrs;

public interface IHandlerBase<in TRequest, TResult> : IRequestHandler<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    Task<TResult> IRequestHandler<TRequest, TResult>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return Handle(request, cancellationToken);
    }

    public new async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken)
    {
        using var handlerActivity = ApplicationDiagnostics.StartActivity(GetType().Name);

        return await Process(request, cancellationToken);
    }

    public Task<TResult> Process(TRequest request, CancellationToken cancellationToken);
}
