using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindProjectManagers
    {
        private readonly IGetLateBillablePeople _getProjectManagersWithOpenTimeEntries;
        private readonly ISendReminder _sendReminder;
        public RemindProjectManagers(IGetLateBillablePeople getProjectManagersWithOpenTimeEntries, ISendReminder sendReminder)
        {
            _getProjectManagersWithOpenTimeEntries = getProjectManagersWithOpenTimeEntries;
            _sendReminder = sendReminder;
        }
        
        public void Execute(RemindLateBillablePeopleRequest remindProjectManagersRequest)
        {
            var projectManagersWithOpenTimeEntries = _getProjectManagersWithOpenTimeEntries.Execute();

            foreach (var projectManager in projectManagersWithOpenTimeEntries.BillablePeople)
            {
                _sendReminder.Execute(new SendReminderRequest
                {
                    Channel = projectManager.Id,
                    Text = remindProjectManagersRequest.Message,
                    Email = projectManager.Email
                });
            }
        }
    }
}
