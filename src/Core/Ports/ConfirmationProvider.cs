using Core.Domain;
using Core.Dtos;

namespace Core.Ports;

public interface ConfirmationProvider
{
    Task<Result<Confirmation>> Begin(ConfirmableAction action, Account account);

    Task<Result<Confirmation>> Finish(ConfirmationDto dto, string code);
}
