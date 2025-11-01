using DevKit.MediatR.Cqrs;

namespace DevKit.Example.Application;

public sealed record ExampleQuery(string TodoId) : IQuery<string>;
