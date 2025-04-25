using Core.Commands.Outputs;
using Core.Domain;

namespace Core.Ports;

public interface SessionCreator
{
    public Result<TokenPairOutput> CreateSession(Account account);
}
