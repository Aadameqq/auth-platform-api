namespace Core.Domain;

public class ConfirmationCode
{
    public readonly ConfirmableAction Action;
    public readonly string Code;
    private readonly DateTime expiresAt;
    public readonly Guid Id = Guid.NewGuid();
    public readonly TimeSpan LifeSpan = TimeSpan.FromMinutes(60);
    public readonly Guid OwnerId;

#pragma warning disable CS8618
    private ConfirmationCode() { }
#pragma warning restore CS8618

    public ConfirmationCode(Account owner, string code, DateTime now, ConfirmableAction action)
    {
        OwnerId = owner.Id;
        expiresAt = now + LifeSpan;
        Code = code;
        Action = action;
    }

    public bool HasExpired(DateTime now)
    {
        return now >= expiresAt;
    }

    public bool IsOwner(Account owner)
    {
        return OwnerId == owner.Id;
    }
}
