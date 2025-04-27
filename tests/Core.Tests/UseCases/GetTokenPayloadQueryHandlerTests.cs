using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;
using Core.Queries;
using Moq;

namespace Core.Tests.UseCases;

public class GetTokenPayloadQueryHandlerTests
{
    private readonly GetTokenPayloadQueryHandler queryHandler;
    private readonly AccessTokenPayload testPayload = new(Guid.Empty, Guid.Empty, Role.None);
    private readonly Mock<AuthTokenService> tokenServiceMock = new();

    private readonly string validToken = "token";

    public GetTokenPayloadQueryHandlerTests()
    {
        queryHandler = new GetTokenPayloadQueryHandler(tokenServiceMock.Object);

        tokenServiceMock.Setup(x => x.FetchPayloadIfValid(validToken)).ReturnsAsync(testPayload);
    }

    [Fact]
    public async Task WhenTokenInvalid_ShouldFail()
    {
        var result = await queryHandler.Execute("invalid-token");

        Assert.True(result.IsFailure);
        Assert.IsType<InvalidToken>(result.Exception);
    }

    [Fact]
    public async Task WhenTokenValid_ShouldSucceedAndReturnTokenPayload()
    {
        var result = await queryHandler.Execute(validToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(testPayload, result.Value);
    }
}
