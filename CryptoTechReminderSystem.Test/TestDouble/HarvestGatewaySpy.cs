using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class HarvestGatewaySpy : IHarvestBillablePersonRetriever, ITimeSheetRetriever
    {
        public bool IsRetrieveBillablePeopleCalled;
        public bool IsRetrieveTimeSheetsCalled;
        public DateTimeOffset[] RetrieveTimeSheetsArguments;
            
        public IList<HarvestBillablePerson> RetrieveBillablePeople()
        {
            IsRetrieveBillablePeopleCalled = true;
              
            return new List<HarvestBillablePerson>();
        }

        public IList<TimeSheet> RetrieveTimeSheets(DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            IsRetrieveTimeSheetsCalled = true;
            
            RetrieveTimeSheetsArguments = new[]
            {
                dateFrom,
                dateTo
            };

            return new List<TimeSheet>();
        }
    }
}
