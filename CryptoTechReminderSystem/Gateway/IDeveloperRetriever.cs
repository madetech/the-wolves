using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface IDeveloperRetriever
    {
        Developer Retrieve();
    }
}