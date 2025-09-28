using System.Diagnostics;
using DevKit.Base;
using DevKit.MediatR.Cqrs;
using ErrorOr;
using MediatR;

namespace DevKit.MediatR.Pipelines;

public sealed class UnitOfWorkBehaviour<TCommand, TResult> : IPipelineBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : IErrorOr
{
    private const string BaseActivityName = nameof(UnitOfWorkBehaviour<TCommand, TResult>);
    private const string PreActivityName = $"{BaseActivityName}.Pre";
    private const string PostActivityName = $"{BaseActivityName}.Post";

    private readonly IDevKitDbContext _devKitDbContext;

    public UnitOfWorkBehaviour(IDevKitDbContext devKitDbContext)
    {
        _devKitDbContext = devKitDbContext;
    }

    public async Task<TResult> Handle(TCommand request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        using var preActivity = ApplicationDiagnostics.StartActivity(PreActivityName);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        await using var transaction = await _devKitDbContext.BeginTransactionAsync(cancellationToken);
        try
        {
            preActivity?.FinishActivity();
            var result = await next(cancellationToken);

            using var postActivity = ApplicationDiagnostics.StartActivity(PostActivityName);
            postActivity?.AddEvent(new ActivityEvent("Commit start"));
            await transaction.CommitAsync(cancellationToken);
            postActivity?.AddEvent(new ActivityEvent("Commit complete"));
            return result;
        }
        catch (Exception ex)
        {
            using var postActivity = ApplicationDiagnostics.StartActivity(PostActivityName);
            postActivity?.SetFailure(ex);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
