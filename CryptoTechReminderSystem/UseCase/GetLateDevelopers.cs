using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetLateBillablePeople : IGetLateBillablePeople
    {
        private readonly IHarvestBillablePersonRetriever _harvestBillablePersonRetriever;
        private readonly ITimeSheetRetriever _harvestTimeSheetRetriever;
        private readonly ISlackBillablePersonRetriever _slackBillablePersonRetriever;
        private readonly IClock _clock;

        public GetLateBillablePeople(ISlackBillablePersonRetriever slackBillablePersonRetriever, IHarvestBillablePersonRetriever harvestBillablePersonRetriever, ITimeSheetRetriever harvestTimeSheetRetriever, IClock clock)
        {
            _harvestBillablePersonRetriever = harvestBillablePersonRetriever;
            _harvestTimeSheetRetriever = harvestTimeSheetRetriever;
            _slackBillablePersonRetriever = slackBillablePersonRetriever;
            _clock = clock;
        }
        
        public GetLateBillablePeopleResponse Execute()
        {
            var getLateBillablePeopleResponse = new GetLateBillablePeopleResponse
            {
                BillablePeople = new List<GetLateBillablePeopleResponse.LateBillablePerson>()
            };
            
            if (IsWeekend(_clock.Now())) return getLateBillablePeopleResponse;
            
            var harvestGetBillablePeopleResponse = _harvestBillablePersonRetriever.RetrieveBillablePeople();
            var slackGetBillablePeopleResponse = _slackBillablePersonRetriever.RetrieveBillablePeople();

            var dateFrom = GetStartingDate(_clock.Now());
            var dateTo = GetEndingDate(_clock.Now());
            
            var harvestGetTimeSheetsResponse = _harvestTimeSheetRetriever.RetrieveTimeSheets(dateFrom, dateTo);
            
            foreach (var harvestBillablePerson in harvestGetBillablePeopleResponse)
            {
                var timeSheetForBillablePerson = harvestGetTimeSheetsResponse.Where(sheet => sheet.UserId == harvestBillablePerson.Id);
                var sumOfHours = timeSheetForBillablePerson.Sum(timeSheet => timeSheet.Hours);
                
                if (sumOfHours < ExpectedHoursByDate(harvestBillablePerson.WeeklyHours, _clock.Now()))
                {
                    var slackLateBillablePerson = slackGetBillablePeopleResponse.SingleOrDefault(billablePerson => String.Equals(RemoveTopLevelDomain(billablePerson.Email), RemoveTopLevelDomain(harvestBillablePerson.Email), StringComparison.OrdinalIgnoreCase));
                    
                    if (slackLateBillablePerson != null)
                    {
                        getLateBillablePeopleResponse.BillablePeople.Add(new GetLateBillablePeopleResponse.LateBillablePerson
                        {
                            Id = slackLateBillablePerson.Id,
                            Email = slackLateBillablePerson.Email
                        });
                    }
                }
            }
            return getLateBillablePeopleResponse;
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
