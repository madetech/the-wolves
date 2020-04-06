using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class HarvestGatewayStub : IHarvestBillablePersonRetriever, ITimeSheetRetriever
    {
        public HarvestBillablePerson[] BillablePeople { private get; set; }
            
        public TimeSheet[] TimeSheets { private get; set; }
            
        public IList<HarvestBillablePerson> RetrieveBillablePeople()
        {
            return BillablePeople;
        }

        public IList<TimeSheet> RetrieveTimeSheets(DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            return TimeSheets;
        }
    }
}
