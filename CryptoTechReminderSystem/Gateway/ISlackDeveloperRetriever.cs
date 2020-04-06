using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface ISlackBillablePersonRetriever
    {
        IList<SlackBillablePerson> RetrieveBillablePeople();
    }
}
