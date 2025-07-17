using Core.Commands;
using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;
using NSubstitute;

namespace Core.Tests.UseCases;

public class UnassignRoleCommandHandlerTests
{
    private readonly AccountsRepository accountsRepositoryMock =
        Substitute.For<AccountsRepository>();

    private readonly UnassignRoleCommandHandler commandHandler;

    private readonly Account existingAccount = new("userName", "email", "password");
    private readonly UnitOfWork uowMock = Substitute.For<UnitOfWork>();

    public UnassignRoleCommandHandlerTests()
    {
        commandHandler = new UnassignRoleCommandHandler(uowMock);

        uowMock.GetAccountsRepository().Returns(accountsRepositoryMock);

        accountsRepositoryMock.FindById(existingAccount.Id).Returns(existingAccount);
    }

    [Fact]
    public async Task ShouldFail_WhenAccountWithGivenIdDoesNotExist()
    {
        var result = await RunCommand(Guid.NewGuid(), Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch<Account>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task ShouldFail_WhenAccountAttemptsToSelfUnassignRole()
    {
        var result = await RunCommand(existingAccount.Id, existingAccount.Id);

        Assert.True(result.IsFailure);
        Assert.IsType<CannotManageOwn<Role>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task ShouldSucceed_WhenAccountWithGivenIdExistsAndIsNotIssuer()
    {
        var result = await RunCommand(Guid.NewGuid(), existingAccount.Id);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ShouldPersistCorrectAccount_WhenAccountWithGivenIdExistsAndIsNotIssuer()
    {
        await RunCommand(Guid.NewGuid(), existingAccount.Id);

        await accountsRepositoryMock
            .Received()
            .Update(Arg.Is<Account>(a => a.Id == existingAccount.Id));
    }

    private Task<Result> RunCommand(Guid issuerId, Guid accountId)
    {
        var cmd = new UnassignRoleCommand(issuerId, accountId);
        return commandHandler.Handle(cmd, CancellationToken.None);
    }

    private void AssertNoChanges()
    {
        uowMock.DidNotReceive().Flush();
    }
}
