using System.Collections.Generic;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface ITimeSheetRetriever
    {
        IList<TimeSheet> RetrieveTimeSheets();
    }
}