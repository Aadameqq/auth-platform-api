using Core.Dtos;

namespace Api.Controllers.Dtos;

public record GetAuthenticatedUserResponse
{
    public GetAuthenticatedUserResponse(AccountDto account)
    {
        Id = account.Id;
        UserName = account.UserName;
        Email = account.Email;
    }

    public string Email { get; private init; }
    public Guid Id { get; private init; }
    public string UserName { get; private init; }
}
