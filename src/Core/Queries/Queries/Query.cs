using Core.Domain;
using MediatR;

namespace Core.Queries.Queries;

public interface Query<TOutput> : IRequest<Result<TOutput>> { }
