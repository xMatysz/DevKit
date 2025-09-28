using DevKit.MediatR.Cqrs;
using ErrorOr;

namespace DevKit.Example.Application;

public sealed class ExampleCommandHandler : ICommandHandler<ExampleCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Process(ExampleCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(3_000, cancellationToken);
        return request.Test;
    }
}
