namespace DevKit.MediatR.Cqrs;

public interface IQueryHandler<in TQuery, TResult> : IHandlerBase<TQuery, TResult>
    where TQuery : IQuery<TResult>;
