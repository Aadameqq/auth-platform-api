namespace Core.Queries.Queries;

public record HasAccountBeenActivatedQuery(Guid Id) : Query<bool>;
