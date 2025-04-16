using Core.Ports;

namespace Infrastructure.Other;

public class SystemDateTimeProvider : DateTimeProvider
{
    public DateTime Now()
    {
        return DateTime.UtcNow;
    }
}
