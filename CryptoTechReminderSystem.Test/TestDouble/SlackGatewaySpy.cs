using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class SlackGatewaySpy : ISlackDeveloperRetriever
    {
        public bool IsRetrieveBillablePeopleCalled;

        public IList<SlackDeveloper> RetrieveBillablePeople()
        {
            IsRetrieveBillablePeopleCalled = true;
                
            return new List<SlackDeveloper>();
        }
    }
}
