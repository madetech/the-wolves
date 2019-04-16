using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetLateDevelopers : IGetLateDevelopers
    {
        private readonly IHarvestDeveloperRetriever _harvestDeveloperRetriever;
        private readonly ITimeSheetRetriever _harvestTimeSheetRetriever;
        private readonly ISlackDeveloperRetriever _slackDeveloperRetriever;
        private IClock _clock;

        public GetLateDevelopers(ISlackDeveloperRetriever slackDeveloperRetriever, IHarvestDeveloperRetriever harvestDeveloperRetriever, ITimeSheetRetriever harvestTimeSheetRetriever, IClock clock)
        {
            _harvestDeveloperRetriever = harvestDeveloperRetriever;
            _harvestTimeSheetRetriever = harvestTimeSheetRetriever;
            _slackDeveloperRetriever = slackDeveloperRetriever;
            _clock = clock;
        }
        
        public GetLateDevelopersResponse Execute()
        {
            var harvestGetDevelopersResponse = _harvestDeveloperRetriever.RetrieveDevelopers();
            var slackGetDevelopersResponse = _slackDeveloperRetriever.RetrieveDevelopers();

            DateTimeOffset dateFrom = GetStartingDate(_clock.Now());
            DateTimeOffset dateTo = GetEndingDate(_clock.Now());
            
            var harvestGetTimesheetsResponse = _harvestTimeSheetRetriever.RetrieveTimeSheets(dateFrom, dateTo);
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
        
        private static DateTimeOffset GetStartingDate(DateTimeOffset currentDateTime)
        {
            var diff = (7 + (currentDateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            return currentDateTime.AddDays(-diff);
        }

        private static DateTimeOffset GetEndingDate(DateTimeOffset currentDateTime)
        {
            var diff = (7 + (DayOfWeek.Friday - currentDateTime.DayOfWeek)) % 7;
            return currentDateTime.AddDays(diff);
        }
    }
}
