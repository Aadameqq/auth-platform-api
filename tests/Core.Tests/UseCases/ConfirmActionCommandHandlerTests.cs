// using Core.Commands;
// using Core.Commands.Commands;
// using Core.Domain;
// using Core.Exceptions;
// using Core.Ports;
// using NSubstitute;
//
// namespace Core.Tests.UseCases;
//
// public class ConfirmActionCommandHandlerTests
// {
//     private readonly AccountsRepository accountsRepositoryMock =
//         Substitute.For<AccountsRepository>();
//
//     private readonly ActivationCodesRepository activationCodesRepositoryMock =
//         Substitute.For<ActivationCodesRepository>();
//
//     private readonly ConfirmActionCommandHandler commandHandler;
//
//     private readonly Account existingAccount = new("userName", "email", "password");
//     private readonly string existingCode = "123";
//
//     private readonly UnitOfWork uowMock = Substitute.For<UnitOfWork>();
//
//     public ConfirmActionCommandHandlerTests()
//     {
//         commandHandler = new ConfirmActionCommandHandler(activationCodesRepositoryMock, uowMock);
//
//         activationCodesRepositoryMock
//             .GetAccountIdAndRevokeCode(existingCode)
//             .Returns(existingAccount.Id);
//
//         accountsRepositoryMock.FindById(existingAccount.Id).Returns(existingAccount);
//
//         uowMock.GetAccountsRepository().Returns(accountsRepositoryMock);
//     }
//
//     [Fact]
//     public async Task WhenCodeDoesNotExist_ShouldFail()
//     {
//         var result = await RunCommand("invalid-code");
//
//         Assert.True(result.IsFailure);
//         Assert.IsType<NoSuch>(result.Exception);
//         AssertNoChanges();
//     }
//
//     [Fact]
//     public async Task WhenUserAssignedToCodeDoesNotExist_ShouldFail()
//     {
//         accountsRepositoryMock.FindById(existingAccount.Id).Returns((Account)null!);
//
//         var result = await RunCommand(existingCode);
//
//         Assert.True(result.IsFailure);
//         Assert.IsType<NoSuch<Account>>(result.Exception);
//         AssertNoChanges();
//     }
//
//     [Fact]
//     public async Task WhenUserHasAlreadyActivated_ShouldFail()
//     {
//         existingAccount.Activate();
//         var result = await RunCommand(existingCode);
//
//         Assert.True(result.IsFailure);
//         Assert.IsType<NoSuch<Account>>(result.Exception);
//         AssertNoChanges();
//     }
//
//     [Fact]
//     public async Task WhenCodeIsValid_ShouldActivateAccountAndPersistIt()
//     {
//         var result = await RunCommand(existingCode);
//
//         Assert.True(result.IsSuccess);
//         await accountsRepositoryMock
//             .Received()
//             .Update(Arg.Is<Account>(a => a.Id == existingAccount.Id && a.HasBeenActivated()));
//     }
//
//     private void AssertNoChanges()
//     {
//         uowMock.DidNotReceive().Flush();
//         activationCodesRepositoryMock.DidNotReceive().Create(Arg.Any<Account>());
//     }
//
//     private Task<Result> RunCommand(string code)
//     {
//         var cmd = new ConfirmActionCommand(code);
//         return commandHandler.Handle(cmd, CancellationToken.None);
//     }
// }
