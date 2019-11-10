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
            var getLateDevelopersResponse = new GetLateDevelopersResponse
            {
                Developers = new List<GetLateDevelopersResponse.LateDeveloper>()
            };
            
            if (IsWeekend(_clock.Now())) return getLateDevelopersResponse;
            
            var harvestGetDevelopersResponse = _harvestDeveloperRetriever.RetrieveDevelopers();
            var slackGetDevelopersResponse = _slackDeveloperRetriever.RetrieveDevelopers();

            var dateFrom = GetStartingDate(_clock.Now());
            var dateTo = GetEndingDate(_clock.Now());
            
            var harvestGetTimeSheetsResponse = _harvestTimeSheetRetriever.RetrieveTimeSheets(dateFrom, dateTo);
            
            foreach (var harvestDeveloper in harvestGetDevelopersResponse)
            {
                var timeSheetForDeveloper = harvestGetTimeSheetsResponse.Where(sheet => sheet.UserId == harvestDeveloper.Id);
                var sumOfHours = timeSheetForDeveloper.Sum(timeSheet => timeSheet.Hours);
                
                if (sumOfHours < ExpectedHoursByDate(harvestDeveloper.WeeklyHours, _clock.Now()))
                {
                    var slackLateDeveloper = slackGetDevelopersResponse.SingleOrDefault(developer => String.Equals(RemoveTopLevelDomain(developer.Email), RemoveTopLevelDomain(harvestDeveloper.Email), StringComparison.OrdinalIgnoreCase));
                    
                    if (slackLateDeveloper != null)
                    {
                        getLateDevelopersResponse.Developers.Add(new GetLateDevelopersResponse.LateDeveloper
                        {
                            Id = slackLateDeveloper.Id,
                            Email = slackLateDeveloper.Email
                        });
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
            if (IsEndOfTheMonth(currentDateTime)) return currentDateTime;
            
            var daysToFriday = (7 + (DayOfWeek.Friday - currentDateTime.DayOfWeek)) % 7;
            
            return currentDateTime.AddDays(daysToFriday);
        }

        private static string RemoveTopLevelDomain(string email)
        {
            return email.Replace(".co.uk", "").Replace(".com","");
        }

        private static bool IsEndOfTheMonth(DateTimeOffset currentDateTime)
        {
            return DateTime.DaysInMonth(currentDateTime.Year, currentDateTime.Month) == currentDateTime.Day;
        }
        
        private static bool IsWeekend(DateTimeOffset currentDateTime)
        {
            var weekendDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weekendDays.Contains(currentDateTime.DayOfWeek);
        }
        
        private static int ExpectedHoursByDate(int weeklyHours, DateTimeOffset currentDateTime)
        {
            var nonWorkingWeekDaysForPartTimeContract = 5 - weeklyHours / 7;
            var workingDaysByToday = currentDateTime.DayOfWeek - DayOfWeek.Monday + 1;

            return (workingDaysByToday - nonWorkingWeekDaysForPartTimeContract) * 7;
        }
    }
}
