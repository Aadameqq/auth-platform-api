using Core.Domain;
using MediatR;

namespace Core.Commands.Commands;

public interface Command<TOutput> : IRequest<Result<TOutput>> { }

public interface Command : IRequest<Result> { }
