using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindProjectManagers
    {
        private readonly IGetLateBillablePeople _getProjectManagersWithOpenTimeEntries;
        private readonly ISendReminder _sendReminder;

        private bool _testing;
        public RemindProjectManagers(IGetLateBillablePeople getProjectManagersWithOpenTimeEntries, ISendReminder sendReminder)
        {
            _getProjectManagersWithOpenTimeEntries = getProjectManagersWithOpenTimeEntries;
            _sendReminder = sendReminder;

            _testing = true;
        }
        
        public void Execute(RemindLateBillablePeopleRequest remindProjectManagersRequest)
        {
            if (_testing) {
                TestExecute(remindProjectManagersRequest);
            } else {
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

        private void TestExecute(RemindLateBillablePeopleRequest remindProjectManagersRequest) {
            var ZackSlackUserId = "U019SBSKPSB";
            var projectManagersWithOpenTimeEntries = _getProjectManagersWithOpenTimeEntries.Execute();

            foreach (var projectManager in projectManagersWithOpenTimeEntries.BillablePeople)
            {
                _sendReminder.Execute(new SendReminderRequest
                {
                    Channel = ZackSlackUserId,
                    Text = projectManager.Email + " would have been reminded to approve a time sheet just now.",
                    Email = projectManager.Email
                });
            }
        }
    }
}
