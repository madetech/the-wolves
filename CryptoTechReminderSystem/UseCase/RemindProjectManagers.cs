using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindProjectManagers
    {
        public RemindProjectManagers(IGetLateBillablePeople getProjectManagersWithOpenTimeEntries, ISendReminder sendReminder)
        {
        }
        
        public void Execute(RemindLateBillablePeopleRequest remindLateBillablePeopleRequest)
        {
        }
    }
}
