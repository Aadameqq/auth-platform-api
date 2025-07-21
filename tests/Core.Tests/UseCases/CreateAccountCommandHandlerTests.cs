using Core.Commands;
using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;
using NSubstitute;

namespace Core.Tests.UseCases;

public class CreateAccountCommandHandlerTests
{
    private readonly AccountsRepository accountsRepositoryMock =
        Substitute.For<AccountsRepository>();

    private readonly ActivationCodesRepository codesRepositoryMock =
        Substitute.For<ActivationCodesRepository>();

    private readonly CreateAccountCommandHandler commandHandler;

    private readonly ActivationCodeEmailSender emailSenderMock =
        Substitute.For<ActivationCodeEmailSender>();

    private readonly Account existingAccount = new("userName", "email", "password");
    private readonly string expectedCode = "code";

    private readonly PasswordHasher passwordHasherMock = Substitute.For<PasswordHasher>();

    private readonly TestAccount testAccount = new("new-userName", "new-email", "new-password");
    private readonly UnitOfWork uowMock = Substitute.For<UnitOfWork>();

    public CreateAccountCommandHandlerTests()
    {
        commandHandler = new CreateAccountCommandHandler(
            uowMock,
            passwordHasherMock,
            codesRepositoryMock,
            emailSenderMock
        );

        uowMock.GetAccountsRepository().Returns(accountsRepositoryMock);

        accountsRepositoryMock.FindByEmail(existingAccount.Email).Returns(existingAccount);

        passwordHasherMock
            .HashPassword(Arg.Any<string>())
            .Returns(args => GenerateTestHash(args.Arg<string>()));

        codesRepositoryMock.Create(Arg.Any<Account>()).Returns(expectedCode);
    }

    [Fact]
    public async Task WhenAccountWithGivenEmailAlreadyExists_ShouldFail()
    {
        var result = await RunCommand("test-username", existingAccount.Email, "test-password");

        Assert.True(result.IsFailure);
        Assert.IsType<AlreadyExists<Account>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task WhenGivenEmailIsNotOccupied_ShouldSucceed()
    {
        var result = await RunCommand("new-userName", "new-email", "new-password");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task WhenGivenEmailIsNotOccupied_ShouldPersistAccountWithCorrectData()
    {
        await RunCommand(testAccount.UserName, testAccount.Email, testAccount.Password);

        await accountsRepositoryMock.Received().Create(Arg.Is<Account>(a => IsExpectedAccount(a)));
    }

    [Fact]
    public async Task WhenGivenEmailIsNotOccupied_ShouldCreateCodeForCorrectAccount()
    {
        await RunCommand(testAccount.UserName, testAccount.Email, testAccount.Password);

        await codesRepositoryMock.Received().Create(Arg.Is<Account>(a => IsExpectedAccount(a)));
    }

    [Fact]
    public async Task WhenGivenEmailIsNotOccupied_ShouldSendEmailWithValidData()
    {
        await RunCommand(testAccount.UserName, testAccount.Email, testAccount.Password);

        await emailSenderMock
            .Received()
            .Send(Arg.Is<Account>(a => IsExpectedAccount(a)), Arg.Is(expectedCode));
    }

    private bool IsExpectedAccount(Account? account)
    {
        return account is not null
            && account.UserName == testAccount.UserName
            && account.Email == testAccount.Email
            && account.Password == GenerateTestHash(testAccount.Password);
    }

    private void AssertNoChanges()
    {
        uowMock.DidNotReceive().Flush();
        codesRepositoryMock.DidNotReceive().Create(Arg.Any<Account>());
        emailSenderMock.DidNotReceive().Send(Arg.Any<Account>(), Arg.Any<string>());
    }

    private Task<Result> RunCommand(string userName, string email, string password)
    {
        var cmd = new CreateAccountCommand(userName, email, password);
        return commandHandler.Handle(cmd, CancellationToken.None);
    }

    private string GenerateTestHash(string plainPassword)
    {
        return plainPassword + "hash";
    }

    private record TestAccount(string UserName, string Email, string Password);
}
