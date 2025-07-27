using Core.Exceptions;

namespace Core.Domain;

public class ConfirmationCode
{
    public readonly ConfirmableAction Action;
    private readonly DateTime createdAt;
    public readonly Guid Id = Guid.NewGuid();
    public readonly TimeSpan LifeSpan = TimeSpan.FromMinutes(60);
    public readonly Guid OwnerId;
    public readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);

#pragma warning disable CS8618
    private ConfirmationCode() { }
#pragma warning restore CS8618

    public ConfirmationCode(Account owner, string code, DateTime now, ConfirmableAction action)
    {
        OwnerId = owner.Id;
        createdAt = now;
        Code = code;
        Action = action;
    }

    public string Code { get; private set; }

    public bool HasExpired(DateTime now)
    {
        return now >= createdAt + LifeSpan;
    }

    public bool IsCooldown(DateTime now)
    {
        return now >= createdAt + LifeSpan;
    }

    public bool IsOwner(Account owner)
    {
        return OwnerId == owner.Id;
    }

    public Result Refresh(string code, DateTime now)
    {
        if (HasExpired(now))
        {
            return new Expired();
        }

        if (IsCooldown(now))
        {
            return new TooManyAttempts();
        }

        Code = code;
        return Result.Success();
    }
}
