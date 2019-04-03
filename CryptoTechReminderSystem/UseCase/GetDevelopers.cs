using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetDevelopers
    {
        private IDeveloperRetriever _requester;

        public GetDevelopers(IDeveloperRetriever harvestRequester)
        {
            _requester = harvestRequester;
        }

        public IList<Developer> Execute()
        {
            var request = _requester.Retrieve();

            return request;
        }
    }
}