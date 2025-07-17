using Core.Domain;
using Core.Queries.Queries;
using MediatR;

namespace Core.Queries;

public interface QueryHandler<in TQuery, TOutput> : IRequestHandler<TQuery, Result<TOutput>>
    where TQuery : Query<TOutput>
    where TOutput : class
{
    public new Task<Result<TOutput>> Handle(
        TQuery query,
        CancellationToken cancellationToken = default
    );
}
