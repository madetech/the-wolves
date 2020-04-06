using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class HarvestGatewayStub : IHarvestDeveloperRetriever, ITimeSheetRetriever
    {
        public HarvestDeveloper[] BillablePeople { private get; set; }
            
        public TimeSheet[] TimeSheets { private get; set; }
            
        public IList<HarvestDeveloper> RetrieveBillablePeople()
        {
            return BillablePeople;
        }

        public IList<TimeSheet> RetrieveTimeSheets(DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            return TimeSheets;
        }
    }
}
