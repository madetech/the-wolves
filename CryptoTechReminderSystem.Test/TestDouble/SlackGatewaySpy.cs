using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class SlackGatewaySpy : ISlackBillablePersonRetriever
    {
        public bool IsRetrieveBillablePeopleCalled;

        public IList<SlackBillablePerson> RetrieveBillablePeople()
        {
            IsRetrieveBillablePeopleCalled = true;
                
            return new List<SlackBillablePerson>();
        }
    }
}
