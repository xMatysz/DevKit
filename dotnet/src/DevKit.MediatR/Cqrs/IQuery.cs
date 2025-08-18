using MediatR;

namespace DevKit.MediatR.Cqrs;

public interface IQuery<out TResult> : IRequest<TResult>;
