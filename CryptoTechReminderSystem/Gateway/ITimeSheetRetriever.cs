using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface ITimeSheetRetriever
    {
        IEnumerable<TimeSheet> RetrieveTimeSheets();
    }
}