using Core.Domain;

namespace Core.Ports;

public interface ConfirmationsRepository
{
    Task Create(Confirmation confirmation);
    Task<Confirmation?> FindById(Guid id);
    Task<Confirmation?> FindByAccount(Account account, ConfirmableAction action);
    Task Delete(Confirmation confirmation);
    Task Update(Confirmation confirmation);
    string GenerateCode();
}
