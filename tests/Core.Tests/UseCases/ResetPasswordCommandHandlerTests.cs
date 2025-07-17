using Core.Commands;
using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;
using NSubstitute;

namespace Core.Tests.UseCases;

public class ResetPasswordCommandHandlerTests
{
    private readonly AccountsRepository accountsRepositoryMock =
        Substitute.For<AccountsRepository>();

    private readonly PasswordResetCodesRepository codesRepositoryMock =
        Substitute.For<PasswordResetCodesRepository>();

    private readonly ResetPasswordCommandHandler commandHandler;

    private readonly Account existingAccount = new("userName", "email", "password");
    private readonly string existingCode = "code";

    private readonly PasswordHasher passwordHasherMock = Substitute.For<PasswordHasher>();

    private readonly string testPassword = "new-password";
    private readonly string testPasswordHashed = "new-password-hash";
    private readonly UnitOfWork uowMock = Substitute.For<UnitOfWork>();

    public ResetPasswordCommandHandlerTests()
    {
        commandHandler = new ResetPasswordCommandHandler(
            passwordHasherMock,
            uowMock,
            codesRepositoryMock
        );

        codesRepositoryMock.GetAccountIdAndRevokeCode(existingCode).Returns(existingAccount.Id);

        accountsRepositoryMock.FindById(existingAccount.Id).Returns(existingAccount);

        passwordHasherMock.HashPassword(testPassword).Returns(testPasswordHashed);

        uowMock.GetAccountsRepository().Returns(accountsRepositoryMock);
    }

    [Fact]
    public async Task WhenGivenCodeIsInvalid_ShouldFail()
    {
        var result = await RunCommand("invalid-code", "some-password");

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task WhenGivenCodeIsValidButAccountAssignedToItDoesNotExist_ShouldFail()
    {
        accountsRepositoryMock.FindById(Arg.Any<Guid>()).Returns(null as Account);

        var result = await RunCommand(existingCode, "some-password");

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task WhenGivenCodeIsValid_ShouldSucceed()
    {
        var result = await RunCommand(existingCode, "new-password");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task WhenGivenCodeIsValid_ShouldChangePasswordAndPersist()
    {
        await RunCommand(existingCode, testPassword);

        await accountsRepositoryMock
            .Received()
            .Update(
                Arg.Is<Account>(a => a.Id == existingAccount.Id && a.Password == testPasswordHashed)
            );
    }

    private void AssertNoChanges()
    {
        uowMock.DidNotReceive().Flush();
        codesRepositoryMock.DidNotReceive().Create(Arg.Any<Account>());
    }

    private Task<Result> RunCommand(string resetCode, string newPassword)
    {
        var cmd = new ResetPasswordCommand(resetCode, newPassword);
        return commandHandler.Handle(cmd, CancellationToken.None);
    }
}
