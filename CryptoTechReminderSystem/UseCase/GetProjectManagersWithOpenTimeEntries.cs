using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;


namespace CryptoTechReminderSystem.UseCase
{
    public class GetProjectManagersWithOpenTimeEntries : IGetLateBillablePeople
    {
        private readonly IHarvestBillablePersonRetriever _harvestBillablePersonRetriever;
        private readonly ITimeSheetRetriever _harvestTimeSheetRetriever;
        private readonly ISlackBillablePersonRetriever _slackBillablePersonRetriever;
        private readonly IClock _clock;
        
        public GetProjectManagersWithOpenTimeEntries(ISlackBillablePersonRetriever slackBillablePersonRetriever, IHarvestBillablePersonRetriever harvestBillablePersonRetriever, ITimeSheetRetriever harvestTimeSheetRetriever, IClock clock) {
            _slackBillablePersonRetriever = slackBillablePersonRetriever;
            _harvestBillablePersonRetriever = harvestBillablePersonRetriever;
            _harvestTimeSheetRetriever = harvestTimeSheetRetriever;
            _clock = clock;
        }
        
        public GetLateBillablePeopleResponse Execute() 
        {
            var dateFrom = GetStartingDate(_clock.Now());
            var dateTo = GetEndingDate(_clock.Now());
            
            var timeSheets = _harvestTimeSheetRetriever.RetrieveTimeSheets(dateFrom, dateTo);
            var billablePeople = _harvestBillablePersonRetriever.RetrieveBillablePeople();
            var slackGetBillablePeopleResponse = _slackBillablePersonRetriever.RetrieveBillablePeople();

            return ProjectManagersWithOpenTimeEntries(timeSheets, billablePeople, slackGetBillablePeopleResponse);
        }

        private GetLateBillablePeopleResponse ProjectManagersWithOpenTimeEntries(IList<DomainObject.TimeSheet> timeSheets, IList<DomainObject.HarvestBillablePerson> billablePeople, IList<DomainObject.SlackBillablePerson> slackGetBillablePeopleResponse) {
            var getProjectManagersWithOpenTimeEntriesResponse = new GetLateBillablePeopleResponse
            {
                BillablePeople = new List<GetLateBillablePeopleResponse.LateBillablePerson>()
            };

            foreach (var timeSheet in timeSheets) {
                if (!timeSheet.IsClosed)
                {
                    foreach (var harvestProjectManager in HarvestProjectManagers(billablePeople, timeSheet))
                    {
                        var slackProjectManager =
                            SlackProjectManager(slackGetBillablePeopleResponse, harvestProjectManager);
                        
                        if (slackProjectManager != null)
                        {
                            getProjectManagersWithOpenTimeEntriesResponse.BillablePeople.Add(new GetLateBillablePeopleResponse.LateBillablePerson
                            {
                                Id = slackProjectManager.Id,
                                Email = slackProjectManager.Email
                            });
                        }
                    }
                }
            }

            return getProjectManagersWithOpenTimeEntriesResponse;
        }

        private IEnumerable<HarvestBillablePerson> HarvestProjectManagers(IEnumerable<HarvestBillablePerson> billablePeople, TimeSheet timeSheet)
        {
            return billablePeople.Where(person => timeSheet.ProjectManagerIds.Contains(person.Id));
        }

        private SlackBillablePerson SlackProjectManager(IList<SlackBillablePerson> slackBillablePeople, HarvestBillablePerson harvestProjectManager)
        {
            return slackBillablePeople.SingleOrDefault(billablePerson => String.Equals(RemoveTopLevelDomain(billablePerson.Email), RemoveTopLevelDomain(harvestProjectManager.Email), StringComparison.OrdinalIgnoreCase));
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

        private static bool IsEndOfTheMonth(DateTimeOffset currentDateTime)
        {
            return DateTime.DaysInMonth(currentDateTime.Year, currentDateTime.Month) == currentDateTime.Day;
        }
        private static string RemoveTopLevelDomain(string email)
        {
            return email.Replace(".co.uk", "").Replace(".com","");
        }
    }
}
