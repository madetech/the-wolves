using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface IHarvestBillablePersonRetriever
    {
        IList<HarvestBillablePerson> RetrieveBillablePeople();
    }
}
