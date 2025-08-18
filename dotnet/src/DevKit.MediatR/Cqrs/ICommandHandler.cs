using MediatR;

namespace DevKit.MediatR.Cqrs;

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : class;
