using MediatR;

namespace DevKit.MediatR.Cqrs;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>;
