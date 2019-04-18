using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class HarvestGatewayStub : IHarvestDeveloperRetriever, ITimeSheetRetriever
    {
        public HarvestDeveloper[] Developers { private get; set; }
            
        public TimeSheet[] TimeSheets { private get; set; }
            
        public IList<HarvestDeveloper> RetrieveDevelopers()
        {
            return Developers;
        }

        public IList<TimeSheet> RetrieveTimeSheets(DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            return TimeSheets;
        }
    }
}