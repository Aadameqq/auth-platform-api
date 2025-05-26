using Core.Commands;
using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;
using NSubstitute;

namespace Core.Tests.UseCases;

public class AssignRoleCommandHandlerTests
{
    private readonly AccountsRepository accountsRepositoryMock =
        Substitute.For<AccountsRepository>();

    private readonly AssignRoleCommandHandler commandHandler;

    private readonly Account existingAccount = new("userName", "email", "password");

    private readonly UnitOfWork unitOfWorkMock = Substitute.For<UnitOfWork>();

    public AssignRoleCommandHandlerTests()
    {
        commandHandler = new AssignRoleCommandHandler(unitOfWorkMock);

        unitOfWorkMock.GetAccountsRepository().Returns(accountsRepositoryMock);

        accountsRepositoryMock.FindById(existingAccount.Id).Returns(existingAccount);
    }

    [Fact]
    public async Task ShouldFail_WhenAccountWithGivenIdDoesNotExist()
    {
        var result = await RunCommand(Guid.NewGuid(), Guid.Empty, Role.Admin);

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch<Account>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task ShouldFail_WhenAccountAttemptsToSelfAssignRole()
    {
        var result = await RunCommand(existingAccount.Id, existingAccount.Id, Role.Admin);

        Assert.True(result.IsFailure);
        Assert.IsType<CannotManageOwn<Role>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task ShouldFail_WhenAccountAlreadyHasRole()
    {
        existingAccount.AssignRole(Role.ProblemsCreator, Guid.NewGuid());

        var result = await RunCommand(Guid.NewGuid(), existingAccount.Id, Role.Admin);

        Assert.True(result.IsFailure);
        Assert.IsType<RoleAlreadyAssigned>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task ShouldSucceed_WhenAccountExistsAndHasNoRole()
    {
        var result = await RunCommand(Guid.NewGuid(), existingAccount.Id, Role.Admin);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ShouldPersistAccount_WhenAccountExistsAndHasNoRole()
    {
        await RunCommand(Guid.NewGuid(), existingAccount.Id, Role.Admin);

        accountsRepositoryMock.Received().Update(Arg.Is<Account>(a => a.Id == existingAccount.Id));
        unitOfWorkMock.Received().Flush();
    }

    private Task<Result> RunCommand(Guid issuerId, Guid accountId, Role role)
    {
        var testCmd = new AssignRoleCommand(issuerId, accountId, role);

        return commandHandler.Handle(testCmd, CancellationToken.None);
    }

    private void AssertNoChanges()
    {
        unitOfWorkMock.DidNotReceive().Flush();
    }
}
