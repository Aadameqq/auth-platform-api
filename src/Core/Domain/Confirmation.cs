using Core.Exceptions;

namespace Core.Domain;

public class Confirmation
{
    public readonly ConfirmableAction Action;
    public readonly Guid Id = Guid.NewGuid();
    public readonly TimeSpan LifeSpan = TimeSpan.FromMinutes(60);
    public readonly ConfirmationMethod Method;
    public readonly Guid OwnerId;
    public readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);
    private DateTime createdAt;

    public Confirmation(
        Account owner,
        DateTime now,
        ConfirmableAction action,
        ConfirmationMethod method,
        string? code
    )
        : this(owner.Id, now, action, method, code) { }

    private Confirmation(
        Guid ownerId,
        DateTime now,
        ConfirmableAction action,
        ConfirmationMethod method,
        string? code
    )
    {
        OwnerId = ownerId;
        createdAt = now;
        Code = code;
        Action = action;
        Method = method;
    }

#pragma warning disable CS8618
    private Confirmation() { }
#pragma warning restore CS8618

    public string? Code { get; private set; }

    public bool HasExpired(DateTime now)
    {
        return now >= createdAt + LifeSpan;
    }

    public bool IsCooldown(DateTime now)
    {
        return now <= createdAt + Timeout;
    }

    public Result Check(DateTime now, ConfirmationMethod method, ConfirmableAction action)
    {
        if (method != Method || action != Action)
        {
            return new ConfirmationMismatch();
        }

        if (HasExpired(now))
        {
            return new Expired();
        }

        return Result.Success();
    }

    public bool IsOwner(Account account)
    {
        return OwnerId == account.Id;
    }

    public bool IsCodeCorrect(string code)
    {
        return code == Code;
    }

    public Result Refresh(DateTime now, string? code)
    {
        if (IsCooldown(now))
        {
            return new TooManyAttempts();
        }

        if (HasExpired(now))
        {
            return new Expired();
        }

        Code = code;
        createdAt = now;
        return Result.Success();
    }

    public bool DoesMethodEqual(ConfirmationMethod other)
    {
        return Method == other;
    }
}
