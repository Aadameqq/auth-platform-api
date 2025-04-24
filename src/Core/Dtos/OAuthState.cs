namespace Core.Dtos;

public record OAuthState(string Provider)
{
    public readonly Guid Id = Guid.NewGuid();
}
