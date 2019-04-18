using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class SlackGatewaySpy : ISlackDeveloperRetriever
    {
        public bool IsRetrieveDevelopersCalled;

        public IList<SlackDeveloper> RetrieveDevelopers()
        {
            IsRetrieveDevelopersCalled = true;
                
            return new List<SlackDeveloper>();
        }
    }
}