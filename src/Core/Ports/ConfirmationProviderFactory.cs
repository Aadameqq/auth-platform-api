using Core.Domain;

namespace Core.Ports;

public interface ConfirmationProviderFactory
{
    ConfirmationProvider CreateInstance(ConfirmationMethod method);
}
