using DevKit.MediatR.Cqrs;
using ErrorOr;

namespace DevKit.Example.Application;

public sealed class ExampleCommand : ICommand<ErrorOr<string>>
{
    public string Test { get; set; }
}
