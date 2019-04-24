using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindLateDevelopers
    {
        private readonly IGetLateDevelopers _getLateDevelopers;
        private readonly ISendReminder _sendReminder;
        private readonly IClock _clock;

        public RemindLateDevelopers(IGetLateDevelopers getLateDevelopers, ISendReminder sendReminder, IClock clock)
        {
            _getLateDevelopers = getLateDevelopers;
            _sendReminder = sendReminder;
            _clock = clock;
        }
        
        public void Execute(RemindLateDevelopersRequest remindLateDevelopersRequest)
        {
            if (IsHalfHourInterval() && IsBeforeTwoPm() && _clock.Now().DayOfWeek == DayOfWeek.Friday)
            {
                var lateDevelopers = _getLateDevelopers.Execute();
                
                foreach (var lateDeveloper in lateDevelopers.Developers)
                {
                    _sendReminder.Execute(new SendReminderRequest
                    {
                        Channel = lateDeveloper,
                        Text = remindLateDevelopersRequest.Message
                    });
                }
            }
        }
        
        private bool IsHalfHourInterval()
        {
            return _clock.Now().Minute % 30 == 0;
        }

        private bool IsBeforeTwoPm()
        {
            return _clock.Now().Hour < 14;
        }
    }
}
