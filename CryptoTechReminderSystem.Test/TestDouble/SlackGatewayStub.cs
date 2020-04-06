using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class SlackGatewayStub : ISlackBillablePersonRetriever
    {
        public SlackBillablePerson[] BillablePeople;

        public IList<SlackBillablePerson> RetrieveBillablePeople()
        {
            return BillablePeople;
        }
    }
}
