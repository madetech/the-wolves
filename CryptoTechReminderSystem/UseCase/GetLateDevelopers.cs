using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetLateDevelopers
    {
        private readonly IMessageSenderAndRetriever _slackGateway;
        private readonly IHarvestDeveloperRetriever _harvestDeveloperRetriever;
        private readonly ITimeSheetRetriever _harvestTimeSheetRetriever;

        public GetLateDevelopers(IMessageSenderAndRetriever slackGateway, IHarvestDeveloperRetriever harvestDeveloperRetriever, ITimeSheetRetriever harvestTimeSheetRetriever)
        {
            _slackGateway = slackGateway;
            _harvestDeveloperRetriever = harvestDeveloperRetriever;
            _harvestTimeSheetRetriever = harvestTimeSheetRetriever;

        }

        public GetLateDevelopersResponse Execute()
        {
            var harvestGetDevelopersResponse = _harvestDeveloperRetriever.RetrieveDevelopers();
            var slackGetDevelopersResponse = _slackGateway.RetrieveDevelopers();
            var harvestGetTimesheetsResponse = _harvestTimeSheetRetriever.RetrieveTimeSheets();
            var getLateDevelopersResponse = new GetLateDevelopersResponse
            {
                Developers = new List<string>()
            };
            foreach (var harvestDeveloper in harvestGetDevelopersResponse)
            {
                var timeSheetForDeveloper = harvestGetTimesheetsResponse.Where(sheet => sheet.UserId == harvestDeveloper.Id);
                var sumOfHours = timeSheetForDeveloper.Sum(timeSheet => timeSheet.Hours);
                if (sumOfHours < 35)
                {
                    var slackLateDeveloper = slackGetDevelopersResponse.Single(developer => developer.Email == harvestDeveloper.Email);
                    getLateDevelopersResponse.Developers.Add(slackLateDeveloper.Id);
                }
            }
            return getLateDevelopersResponse;
        }
    }
}