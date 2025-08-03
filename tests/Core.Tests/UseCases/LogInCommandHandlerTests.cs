using Core.Commands;
using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;
using NSubstitute;

namespace Core.Tests.UseCases;

public class LogInCommandHandlerTests
{
    private readonly AccountsRepository accountsRepositoryMock =
        Substitute.For<AccountsRepository>();

    private readonly LogInCommandHandler commandHandler;
    private readonly DateTimeProvider dateTimeProviderMock = Substitute.For<DateTimeProvider>();

    private readonly Account existingAccount = new("userName", "email", "password");
    private readonly string existingPlainPassword = "plain-password";
    private readonly TokenPairOutput generatedTokenPair = new("access-token", "refresh-token");

    private readonly PasswordVerifier passwordVerifierMock = Substitute.For<PasswordVerifier>();
    private readonly SessionCreator sessionCreatorMock = Substitute.For<SessionCreator>();
    private readonly UnitOfWork uowMock = Substitute.For<UnitOfWork>();

    public LogInCommandHandlerTests()
    {
        commandHandler = new LogInCommandHandler(uowMock, passwordVerifierMock, sessionCreatorMock);

        accountsRepositoryMock.FindByEmail(existingAccount.Email).Returns(existingAccount);

        passwordVerifierMock.Verify(existingPlainPassword, existingAccount.Password!).Returns(true);

        dateTimeProviderMock.Now().Returns(DateTime.MinValue);

        uowMock.GetAccountsRepository().Returns(accountsRepositoryMock);

        sessionCreatorMock.CreateSession(Arg.Any<Account>()).Returns(generatedTokenPair);
    }

    [Fact]
    public async Task WhenAccountWithGivenEmailDoesNotExist_ShouldFail()
    {
        var result = await RunCommand("invalid-email", existingAccount.Password!);

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch<Account>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task WhenPasswordIsInvalid_ShouldFail()
    {
        var result = await RunCommand(existingAccount.Email, "invalid-password");

        Assert.True(result.IsFailure);
        Assert.IsType<InvalidCredentials>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task WhenCredentialsAreValidAndAccountHasBeenActivated_ShouldReturnCorrectTokens()
    {
        existingAccount.Activate();

        var result = await RunCommand(existingAccount.Email, existingPlainPassword);

        Assert.True(result.IsSuccess);
        Assert.Same(generatedTokenPair.AccessToken, result.Value.AccessToken);
        Assert.Same(generatedTokenPair.RefreshToken, result.Value.RefreshToken);
    }

    [Fact]
    public async Task WhenCredentialsAreValidAndAccountHasBeenActivated_UpdateAccountAndCreateSession()
    {
        existingAccount.Activate();

        await RunCommand(existingAccount.Email, existingPlainPassword);

        sessionCreatorMock
            .Received()
            .CreateSession(Arg.Is<Account>(a => a.Id == existingAccount.Id));

        await accountsRepositoryMock
            .Received()
            .Update(Arg.Is<Account>(a => a.Id == existingAccount.Id));
    }

    private void AssertNoChanges()
    {
        uowMock.DidNotReceive().Flush();
    }

    private Task<Result<TokenPairOutput>> RunCommand(string email, string password)
    {
        var cmd = new LogInCommand(email, password);
        return commandHandler.Handle(cmd, CancellationToken.None);
    }
}
