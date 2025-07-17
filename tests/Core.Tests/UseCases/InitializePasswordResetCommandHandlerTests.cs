using Core.Commands;
using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;
using NSubstitute;

namespace Core.Tests.UseCases;

public class InitializePasswordResetCommandHandlerTests
{
    private readonly AccountsRepository accountsRepositoryMock =
        Substitute.For<AccountsRepository>();

    private readonly PasswordResetCodesRepository codesRepositoryMock =
        Substitute.For<PasswordResetCodesRepository>();

    private readonly InitializePasswordResetCommandHandler commandHandler;

    private readonly PasswordResetEmailSender emailSenderMock =
        Substitute.For<PasswordResetEmailSender>();

    private readonly Account existingAccount = new("userName", "email", "password");
    private readonly string existingCode = "code";
    private readonly UnitOfWork uowMock = Substitute.For<UnitOfWork>();

    public InitializePasswordResetCommandHandlerTests()
    {
        commandHandler = new InitializePasswordResetCommandHandler(
            uowMock,
            codesRepositoryMock,
            emailSenderMock
        );

        uowMock.GetAccountsRepository().Returns(accountsRepositoryMock);

        accountsRepositoryMock.FindByEmail(existingAccount.Email).Returns(existingAccount);

        codesRepositoryMock.Create(Arg.Any<Account>()).Returns(existingCode);
    }

    [Fact]
    public async Task WhenAccountWithGivenEmailDoesNotExist_ShouldFail()
    {
        var result = await RunCommand("invalid-email");

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch<Account>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task WhenAccountWithGivenEmailExists_ShouldSucceed()
    {
        var result = await RunCommand(existingAccount.Email);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task WhenAccountWithGivenEmailExists_ShouldCreateCode()
    {
        await RunCommand(existingAccount.Email);

        await codesRepositoryMock
            .Received()
            .Create(Arg.Is<Account>(a => a.Id == existingAccount.Id));
    }

    [Fact]
    public async Task WhenAccountWithGivenEmailExists_ShouldSendEmail()
    {
        await RunCommand(existingAccount.Email);

        await emailSenderMock
            .Received()
            .Send(Arg.Is<Account>(a => a.Id == existingAccount.Id), Arg.Is(existingCode));
    }

    private void AssertNoChanges()
    {
        uowMock.DidNotReceive().Flush();
        codesRepositoryMock.DidNotReceive().Create(Arg.Any<Account>());
        emailSenderMock.DidNotReceive().Send(Arg.Any<Account>(), Arg.Any<string>());
    }

    private Task<Result> RunCommand(string email)
    {
        var cmd = new InitializePasswordResetCommand(email);
        return commandHandler.Handle(cmd, CancellationToken.None);
    }
}
