// using Core.Commands;
// using Core.Domain;
// using Core.Exceptions;
// using Core.Ports;
// using Moq;
//
// namespace Core.Tests.UseCases;
//
// public class LogOutCommandHandlerTests
// {
//     private readonly Mock<AccountsRepository> accountsRepositoryMock = new();
//
//     private readonly Account existingAccount = new("userName", "email", "password");
//
//     private readonly LogOutCommandHandler commandHandler;
//
//     public LogOutCommandHandlerTests()
//     {
//         existingAccount.Activate();
//
//         commandHandler = new LogOutCommandHandler(accountsRepositoryMock.Object);
//
//         accountsRepositoryMock
//             .Setup(x => x.FindById(existingAccount.Id))
//             .ReturnsAsync(existingAccount);
//     }
//
//     [Fact]
//     public async Task WhenAccountWithGivenIdDoesNotExist_ShouldFail()
//     {
//         var result = await commandHandler.Execute(Guid.Empty, Guid.Empty);
//
//         Assert.True(result.IsFailure);
//         Assert.IsType<NoSuch<Account>>(result.Exception);
//         AssertNoChanges();
//     }
//
//     [Fact]
//     public async Task WhenAccountWithGivenIdExists_ShouldSucceedAndRemoveSession()
//     {
//         var testToken = existingAccount.CreateSession(DateTime.MinValue).Value;
//
//         Account? actualAccount = null;
//
//         accountsRepositoryMock
//             .Setup(x => x.UpdateAndFlush(It.IsAny<Account>()))
//             .Callback<Account>(a => actualAccount = a);
//
//         var result = await commandHandler.Execute(existingAccount.Id, testToken.SessionId);
//
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(actualAccount);
//         Assert.Equal(existingAccount.Id, actualAccount.Id);
//         Assert.Null(actualAccount.GetSessionCurrentToken(testToken.SessionId));
//     }
//
//     private void AssertNoChanges()
//     {
//         accountsRepositoryMock.Verify(x => x.UpdateAndFlush(It.IsAny<Account>()), Times.Never);
//     }
// }
