using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface ISlackDeveloperRetriever
    {
        IList<SlackDeveloper> RetrieveDevelopers();
    }
}