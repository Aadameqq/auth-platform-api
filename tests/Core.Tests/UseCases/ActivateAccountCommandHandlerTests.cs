// using Core.Commands;
// using Core.Domain;
// using Core.Exceptions;
// using Core.Ports;
// using Moq;
//
// namespace Core.Tests.UseCases;
//
// public class ActivateAccountCommandHandlerTests
// {
//     private readonly Mock<AccountsRepository> accountsRepositoryMock = new();
//     private readonly Mock<ActivationCodesRepository> activationCodesRepositoryMock = new();
//
//     private readonly ActivateAccountCommandHandler commandHandler;
//
//     private readonly Account existingAccount = new("userName", "email", "password");
//     private readonly string existingCode = "123";
//
//     public ActivateAccountCommandHandlerTests()
//     {
//         commandHandler = new ActivateAccountCommandHandler(
//             activationCodesRepositoryMock.Object,
//             accountsRepositoryMock.Object
//         );
//
//         activationCodesRepositoryMock
//             .Setup(x => x.GetAccountIdAndRevokeCode(existingCode))
//             .ReturnsAsync(existingAccount.Id);
//
//         accountsRepositoryMock
//             .Setup(x => x.FindById(existingAccount.Id))
//             .ReturnsAsync(existingAccount);
//     }
//
//     [Fact]
//     public async Task WhenCodeDoesNotExist_ShouldFail()
//     {
//         var result = await commandHandler.Execute("invalid-code");
//
//         Assert.True(result.IsFailure);
//         Assert.IsType<NoSuch>(result.Exception);
//         AssertNoChanges();
//     }
//
//     [Fact]
//     public async Task WhenUserAssignedToCodeDoesNotExist_ShouldFail()
//     {
//         activationCodesRepositoryMock
//             .Setup(x => x.GetAccountIdAndRevokeCode(existingCode))
//             .ReturnsAsync(Guid.Empty);
//
//         var result = await commandHandler.Execute(existingCode);
//
//         Assert.True(result.IsFailure);
//         Assert.IsType<NoSuch<Account>>(result.Exception);
//         AssertNoChanges();
//     }
//
//     [Fact]
//     public async Task WhenCodeIsValid_ShouldActivateAccountAndPersistIt()
//     {
//         Account? actualAccount = null;
//
//         accountsRepositoryMock
//             .Setup(x => x.UpdateAndFlush(It.IsAny<Account>()))
//             .Callback((Account account) => actualAccount = account);
//
//         var result = await commandHandler.Execute(existingCode);
//
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(actualAccount);
//         Assert.Equal(existingAccount.Id, actualAccount.Id);
//         Assert.True(actualAccount.HasBeenActivated());
//     }
//
//     private void AssertNoChanges()
//     {
//         accountsRepositoryMock.Verify(x => x.UpdateAndFlush(It.IsAny<Account>()), Times.Never);
//     }
// }


