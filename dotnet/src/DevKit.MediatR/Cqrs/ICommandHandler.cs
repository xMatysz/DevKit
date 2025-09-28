using ErrorOr;

namespace DevKit.MediatR.Cqrs;

public interface ICommandHandler<in TCommand, TResult> : IHandlerBase<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : IErrorOr;
