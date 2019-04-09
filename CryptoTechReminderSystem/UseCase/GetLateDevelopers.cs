using System;
using System.Collections.Generic;
using System.Linq;
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
            var harvestLateDevelopersEmails = new List<string>();
            var harvestGetDevelopersResponse = _harvestRequester.RetrieveDevelopers();
            var slackGetDevelopersResponse = _slackRequester.RetrieveDevelopers();
            var harvestGetTimesheetsResponse = _harvestRequester.RetrieveTimeSheets();
            var getLateDevelopersResponse = new GetLateDevelopersResponse()
            {
                Developers = new List<string>()
            };
            foreach (var harvestDeveloper in harvestGetDevelopersResponse)
            {
                
                var timesheetForDeveloper = harvestGetTimesheetsResponse.Where(sheet => sheet.UserId == harvestDeveloper.Id);
                var sumOfHours = timesheetForDeveloper.Sum(timesheet => timesheet.Hours);
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