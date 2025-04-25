using Core.Commands.Commands;
using Core.Domain;

namespace Core.Commands;

public interface CommandHandler<in TCommand, TOutput>
    where TCommand : Command
    where TOutput : class
{
    public Task<Result<TOutput>> Handle(TCommand cmd);
}

public interface CommandHandler<in TCommand>
    where TCommand : Command
{
    public Task<Result> Handle(TCommand command);
}
