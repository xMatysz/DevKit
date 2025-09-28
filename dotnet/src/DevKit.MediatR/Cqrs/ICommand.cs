using ErrorOr;
using MediatR;

namespace DevKit.MediatR.Cqrs;

public interface ICommand<out TResult> : IRequest<TResult>
    where TResult : IErrorOr;
