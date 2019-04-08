using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetLateDevelopers
    {
        private readonly IMessageSenderAndRetriever _slackRequester;
        private readonly ITimesheetAndDeveloperRetriever _harvestRequester;

        public GetLateDevelopers(IMessageSenderAndRetriever slackGateway, ITimesheetAndDeveloperRetriever harvestRequester)
        {
            _slackRequester = slackGateway;
            _harvestRequester = harvestRequester;
        }

        public GetLateDevelopersResponse Execute()
        {
            var getLateDevelopersResponse = new GetLateDevelopersResponse();
            var harvestGetDevelopersResponse = _harvestRequester.RetrieveDevelopers();
            var slackGetDevelopersResponse = _slackRequester.RetrieveDevelopers();
            _harvestRequester.RetrieveTimeSheets();
            return getLateDevelopersResponse;
        }
    }
}