using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetLateDevelopers
    {
        private readonly IDeveloperRetriever _requester;

        public GetLateDevelopers(IMessageSender slackGateway, IDeveloperRetriever harvestRequester)
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