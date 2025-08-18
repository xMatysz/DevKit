using DevKit.MediatR.UnitTests.Doubles;
using NSubstitute;
using Xunit;

namespace DevKit.MediatR.UnitTests;

public class QueryTests : MediatRTestBase
{
    [Fact]
    public async Task SendQuery_Should_Handle_QueryHandler()
    {
        _ = await Mediator.Send(new TestQuery(), TestContext.Current.CancellationToken);

        await QueryHandler.Received(1).Handle(Arg.Any<TestQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendQuery_Should_Not_Handle_CommandHandler()
    {
        _ = await Mediator.Send(new TestQuery(), TestContext.Current.CancellationToken);

        await CommandHandler.DidNotReceiveWithAnyArgs().Handle(null, TestContext.Current.CancellationToken);
    }
}
