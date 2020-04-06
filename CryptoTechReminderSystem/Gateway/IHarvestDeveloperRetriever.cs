using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface IHarvestDeveloperRetriever
    {
        IList<HarvestDeveloper> RetrieveBillablePeople();
    }
}
