using Core.Commands.Commands;
using Core.Domain;
using MediatR;

namespace Core.Commands;

public interface CommandHandler<in TCommand, TOutput> : IRequestHandler<TCommand, Result<TOutput>>
    where TCommand : Command<TOutput>
    where TOutput : class
{
    public new Task<Result<TOutput>> Handle(
        TCommand cmd,
        CancellationToken cancellationToken = default
    );
}

public interface CommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : Command
{
    public new Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}
