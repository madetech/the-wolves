using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class SlackGatewayStub : ISlackDeveloperRetriever
    {
        public SlackDeveloper[] BillablePeople;

        public IList<SlackDeveloper> RetrieveBillablePeople()
        {
            return BillablePeople;
        }
    }
}
