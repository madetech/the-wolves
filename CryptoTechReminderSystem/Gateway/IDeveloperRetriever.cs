using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface IDeveloperRetriever
    {
        IList<Developer> Retrieve();
    }
}