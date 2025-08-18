using MediatR;

namespace DevKit.MediatR.Cqrs;

public interface ICommand : IRequest;
public interface ICommand<out TResult> : IRequest<TResult>;
