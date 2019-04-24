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
        private readonly IClock _clock;

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

            var dateFrom = GetStartingDate(_clock.Now());
            var dateTo = GetEndingDate(_clock.Now());
            
            var harvestGetTimeSheetsResponse = _harvestTimeSheetRetriever.RetrieveTimeSheets(dateFrom, dateTo);
            
            var getLateDevelopersResponse = new GetLateDevelopersResponse
            {
                Developers = new List<string>()
            };
            
            foreach (var harvestDeveloper in harvestGetDevelopersResponse)
            {
                var timeSheetForDeveloper = harvestGetTimeSheetsResponse.Where(sheet => sheet.UserId == harvestDeveloper.Id);
                var sumOfHours = timeSheetForDeveloper.Sum(timeSheet => timeSheet.Hours);
                
                if (sumOfHours < 35)
                {
                    var slackLateDeveloper = slackGetDevelopersResponse.SingleOrDefault(developer => RemoveTopLevelDomain(developer.Email) == RemoveTopLevelDomain(harvestDeveloper.Email));
                    
                    if (slackLateDeveloper != null)
                    {
                        getLateDevelopersResponse.Developers.Add(slackLateDeveloper.Id);
                    }
                }
            }
            
            return getLateDevelopersResponse;
        }
        
        private static DateTimeOffset GetStartingDate(DateTimeOffset currentDateTime)
        {
            var daysFromMonday = (7 + (currentDateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            
            return currentDateTime.AddDays(-daysFromMonday);
        }

        private static DateTimeOffset GetEndingDate(DateTimeOffset currentDateTime)
        {
            var daysToFriday = (7 + (DayOfWeek.Friday - currentDateTime.DayOfWeek)) % 7;
            
            return currentDateTime.AddDays(daysToFriday);
        }

        private static string RemoveTopLevelDomain(string email)
        {
            return email.Replace(".co.uk", "").Replace(".com","");
        }
    }
}
