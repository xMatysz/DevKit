using DevKit.MediatR.Cqrs;

namespace DevKit.Example.Application;

public sealed class ExampleQueryHandler : IQueryHandler<ExampleQuery, string>
{
    public async Task<string> Process(ExampleQuery query, CancellationToken cancellationToken)
    {
        await Task.Delay(2_500, cancellationToken);
        return query.TodoId;
    }
}
