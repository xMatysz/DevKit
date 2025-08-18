using DevKit.MediatR.UnitTests.Doubles;
using NSubstitute;
using Xunit;

namespace DevKit.MediatR.UnitTests;

public class CommandTests : MediatRTestBase
{
    [Fact]
    public async Task SendCommand_Should_Handle_CommandHandler()
    {
        _ = await Mediator.Send(new TestCommand(), TestContext.Current.CancellationToken);

        await CommandHandler.Received(1).Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendCommand_Should_Not_Handle_QueryHandler()
    {
        _ = await Mediator.Send(new TestCommand(), TestContext.Current.CancellationToken);

        await QueryHandler.DidNotReceiveWithAnyArgs().Handle(null, TestContext.Current.CancellationToken);
    }
}
