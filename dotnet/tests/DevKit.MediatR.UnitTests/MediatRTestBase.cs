using DevKit.MediatR.Cqrs;
using DevKit.MediatR.Pipelines;
using DevKit.MediatR.UnitTests.Doubles;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace DevKit.MediatR.UnitTests;

public class MediatRTestBase
{
    public IMediator Mediator { get; }
    public IQueryHandler<TestQuery, object> QueryHandler { get; }
    public ICommandHandler<TestCommand, ErrorOr<object>> CommandHandler { get; }

    public MediatRTestBase()
    {
        QueryHandler = Substitute.For<IQueryHandler<TestQuery, object>>();
        CommandHandler = Substitute.For<ICommandHandler<TestCommand, ErrorOr<object>>>();

        var services = new ServiceCollection()
            .AddSingleton<IRequestHandler<TestQuery, object>>(_ => QueryHandler)
            .AddSingleton<IRequestHandler<TestCommand, ErrorOr<object>>>(_ => CommandHandler)
            .AddDevKitMediatR(typeof(MediatRTestBase).Assembly, typeof(ICommand<>).Assembly)
            .AddDbContext<IDevKitDbContext, TestDbContext>()
            .BuildServiceProvider();

        Mediator = services.GetRequiredService<IMediator>();
    }
}
