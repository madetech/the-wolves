using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetLateDevelopers
    {
        private IDeveloperRetriever _requester;

        public GetLateDevelopers(IDeveloperRetriever harvestRequester)
        {
            _requester = harvestRequester;
        }

        public IList<Developer> Execute()
        {
            var request = _requester.RetrieveDevelopers();

            return request;
        }
    }
}