using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindLateDevelopers
    {
        private readonly IGetLateDevelopers _getLateDevelopers;
        private readonly ISendReminder _sendReminder;

        public RemindLateDevelopers(IGetLateDevelopers getLateDevelopers, ISendReminder sendReminder)
        {
            _getLateDevelopers = getLateDevelopers;
            _sendReminder = sendReminder;
        }
        
        public void Execute(RemindLateDevelopersRequest remindLateDevelopersRequest)
        {
            var lateDevelopers = _getLateDevelopers.Execute();
            
            foreach (var lateDeveloper in lateDevelopers.Developers)
            {
                _sendReminder.Execute(new SendReminderRequest
                {
                    Channel = lateDeveloper.Id,
                    Text = remindLateDevelopersRequest.Message,
                    Email = lateDeveloper.Email
                });
            }
        }
    }
}
