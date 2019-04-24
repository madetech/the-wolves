using System;
using System.Linq;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class ShameLateDevelopers
    {
        private readonly IGetLateDevelopers _getLateDevelopers;
        private readonly ISendReminder _sendReminder;
        private readonly IClock _clock;

        public ShameLateDevelopers(IGetLateDevelopers getLateDevelopers, ISendReminder sendReminder, IClock clock)
        {
            _getLateDevelopers = getLateDevelopers;
            _sendReminder = sendReminder;
            _clock = clock;
        }

        public void Execute(ShameLateDevelopersRequest shameLateDevelopersRequest)
        {
            var currentDateTime = _clock.Now();
            
            if (currentDateTime.Hour == 13 && currentDateTime.Minute == 30 && currentDateTime.DayOfWeek == DayOfWeek.Friday)
            {
                var lateDevelopers = _getLateDevelopers.Execute();
            
                var text = lateDevelopers.Developers.Aggregate(shameLateDevelopersRequest.Message, (current, developer) => current + $"\n• <@{developer}>");

                _sendReminder.Execute(new SendReminderRequest()
                {
                    Text = text,
                    Channel = shameLateDevelopersRequest.Channel
                });
            }
        }
    }
}
