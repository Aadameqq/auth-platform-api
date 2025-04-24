using Core.Domain;
using Core.Dtos;

namespace Core.Ports;

public interface SessionCreator
{
    public Result<TokenPairOutput> CreateSession(Account account);
}
