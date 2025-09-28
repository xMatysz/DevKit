using DevKit.MediatR.Cqrs;

namespace DevKit.Example.Application;

public sealed class ExampleQuery : IQuery<string>
{
    public string TodoId { get; set; }
}
