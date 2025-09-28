using DevKit.MediatR.Cqrs;
using ErrorOr;

namespace DevKit.MediatR.UnitTests.Doubles;

public sealed class TestCommand : ICommand<ErrorOr<object>>;
