using DevKit.MediatR.Cqrs;
using ErrorOr;

namespace DevKit.Example.Application;

public sealed record ExampleCommand(string Test) : ICommand<ErrorOr<string>>;
