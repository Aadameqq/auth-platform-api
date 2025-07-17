namespace Core.Dtos;

public record OAuthState(string Provider)
{
    public readonly Guid Id = Guid.NewGuid();

    public OAuthState(Guid id, string provider)
        : this(provider)
    {
        Id = id;
    }
}
