using Core.Commands;
using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;
using NSubstitute;

namespace Core.Tests.UseCases;

public class LogOutCommandHandlerTests
{
    private readonly AccountsRepository accountsRepositoryMock =
        Substitute.For<AccountsRepository>();

    private readonly LogOutCommandHandler commandHandler;

    private readonly Account existingAccount = new("userName", "email", "password");

    private readonly UnitOfWork unitOfWorkMock = Substitute.For<UnitOfWork>();

    public LogOutCommandHandlerTests()
    {
        existingAccount.Activate();

        commandHandler = new LogOutCommandHandler(unitOfWorkMock);

        unitOfWorkMock.GetAccountsRepository().Returns(accountsRepositoryMock);

        accountsRepositoryMock.FindById(existingAccount.Id).Returns(existingAccount);
    }

    [Fact]
    public async Task WhenAccountWithGivenIdDoesNotExist_ShouldFail()
    {
        var result = await RunCommand(Guid.Empty, Guid.Empty);

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch<Account>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task WhenAccountWithGivenIdExists_ShouldSucceedAndRemoveSession()
    {
        var testToken = existingAccount.CreateSession(DateTime.MinValue).Value;

        var result = await RunCommand(existingAccount.Id, testToken.SessionId);

        Assert.True(result.IsSuccess);
        await accountsRepositoryMock
            .Received()
            .Update(
                Arg.Is<Account>(a =>
                    a.Id == existingAccount.Id
                    && a.GetSessionCurrentToken(testToken.SessionId) == null
                )
            );
        await unitOfWorkMock.Received().Flush();
    }

    private Task<Result> RunCommand(Guid accountId, Guid sessionId)
    {
        var cmd = new LogOutCommand(accountId, sessionId);

        return commandHandler.Handle(cmd, CancellationToken.None);
    }

    private void AssertNoChanges()
    {
        unitOfWorkMock.DidNotReceive().Flush();
    }
}
