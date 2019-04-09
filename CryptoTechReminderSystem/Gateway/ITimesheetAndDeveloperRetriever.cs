using System;
using System.Collections.Generic;

namespace CryptoTechReminderSystem.Gateway
{
    public interface ITimesheetAndDeveloperRetriever : IHarvestDeveloperRetriever, ITimeSheetRetriever
    { 
    }
}