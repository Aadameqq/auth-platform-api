using Core.Commands;
using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;
using NSubstitute;

namespace Core.Tests.UseCases;

public class RefreshTokensCommandHandlerTests
{
    private readonly AccountsRepository accountsRepositoryMock =
        Substitute.For<AccountsRepository>();

    private readonly RefreshTokensCommandHandler commandHandler;
    private readonly DateTimeProvider dateTimeProviderMock = Substitute.For<DateTimeProvider>();

    private readonly Account existingAccount = new("userName", "email", "password");
    private readonly Guid existingCurrentTokenId;
    private readonly Guid existingSessionId;
    private readonly TokenPairOutput existingTokenPair = new("access", "refresh");

    private readonly AuthTokenService tokenServiceMock = Substitute.For<AuthTokenService>();
    private readonly UnitOfWork uowMock = Substitute.For<UnitOfWork>();
    private readonly string validToken = "validToken";

    public RefreshTokensCommandHandlerTests()
    {
        commandHandler = new RefreshTokensCommandHandler(
            uowMock,
            dateTimeProviderMock,
            tokenServiceMock
        );

        existingAccount.Activate();

        var created = existingAccount.CreateSession(DateTime.MinValue).Value;
        existingSessionId = created.SessionId;
        existingCurrentTokenId = created.Id;

        var validTokenPayload = new RefreshTokenPayload(
            existingAccount.Id,
            existingCurrentTokenId,
            existingSessionId
        );

        tokenServiceMock.FetchRefreshTokenPayloadIfValid(validToken).Returns(validTokenPayload);

        accountsRepositoryMock.FindById(existingAccount.Id).Returns(existingAccount);

        tokenServiceMock
            .CreateTokenPair(existingAccount, existingSessionId, Arg.Any<Guid>())
            .Returns(existingTokenPair);

        uowMock.GetAccountsRepository().Returns(accountsRepositoryMock);
    }

    [Fact]
    public async Task ShouldFail_WhenGivenTokenIsInvalid()
    {
        var result = await RunCommand("invalid-token");

        Assert.True(result.IsFailure);
        Assert.IsType<InvalidToken>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task ShouldFail_WhenAccountWithIdFromPayloadDoesNotExist()
    {
        var payloadWithInvalidAccount = new RefreshTokenPayload(
            Guid.Empty,
            existingCurrentTokenId,
            existingSessionId
        );

        tokenServiceMock
            .FetchRefreshTokenPayloadIfValid(validToken)
            .Returns(payloadWithInvalidAccount);

        var result = await RunCommand(validToken);

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch<Account>>(result.Exception);
        AssertNoChanges();
    }

    [Fact]
    public async Task ShouldFail_WhenSessionLinkedToTokenDoesNotExist()
    {
        var payloadWithInvalidSession = new RefreshTokenPayload(
            existingAccount.Id,
            existingCurrentTokenId,
            Guid.Empty
        );

        tokenServiceMock
            .FetchRefreshTokenPayloadIfValid(validToken)
            .Returns(payloadWithInvalidSession);

        var result = await RunCommand(validToken);

        Assert.True(result.IsFailure);
        Assert.IsType<NoSuch<AuthSession>>(result.Exception);
    }

    [Fact]
    public async Task ShouldFail_WhenSessionLinkedToTokenExistsButGivenTokenIsNotCurrentOne()
    {
        var payloadWithTokenNotBeingCurrentOne = new RefreshTokenPayload(
            existingAccount.Id,
            Guid.Empty,
            existingSessionId
        );

        tokenServiceMock
            .FetchRefreshTokenPayloadIfValid(validToken)
            .Returns(payloadWithTokenNotBeingCurrentOne);

        var result = await RunCommand(validToken);

        Assert.True(result.IsFailure);
        Assert.IsType<InvalidToken>(result.Exception);
    }

    [Fact]
    public async Task ShouldSucceedAndReturnCorrectPair_WhenAllRequirementsAreFulfilled()
    {
        var result = await RunCommand(validToken);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equivalent(existingTokenPair, result.Value);
    }

    [Fact]
    public async Task ShouldPersistAccountWithUpdatedSession_WhenAllRequirementsAreFulfilled()
    {
        await RunCommand(validToken);

        var actualToken = existingAccount.GetSessionCurrentToken(existingSessionId);

        await accountsRepositoryMock
            .Received()
            .Update(Arg.Is<Account>(a => a.Id == existingAccount.Id));

        Assert.NotNull(actualToken);

        tokenServiceMock
            .Received()
            .CreateTokenPair(
                Arg.Is<Account>(a => a.Id == existingAccount.Id),
                existingSessionId,
                actualToken.Id
            );
    }

    private void AssertNoChanges()
    {
        uowMock.DidNotReceive().Flush();
    }

    private Task<Result<TokenPairOutput>> RunCommand(string token)
    {
        var cmd = new RefreshTokensCommand(token);
        return commandHandler.Handle(cmd, CancellationToken.None);
    }
}
