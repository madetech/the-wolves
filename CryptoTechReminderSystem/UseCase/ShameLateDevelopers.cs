using System;
using System.Linq;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class ShameLateDevelopers
    {
        private IGetLateDevelopers _getLateDevelopers;
        private IRemindDeveloper _remindDeveloper;
        private IClock _clock;

        public ShameLateDevelopers(IGetLateDevelopers getLateDevelopers, IRemindDeveloper remindDeveloper, IClock clock)
        {
            _getLateDevelopers = getLateDevelopers;
            _remindDeveloper = remindDeveloper;
            _clock = clock;
        }

        public void Execute(ShameLateDevelopersRequest shameLateDevelopersRequest)
        {
            var currentDateTime = _clock.Now();
            
            if (currentDateTime.Hour == 13 && currentDateTime.Minute == 30 && currentDateTime.DayOfWeek == DayOfWeek.Friday)
            {
                var lateDevelopers = _getLateDevelopers.Execute();
            
                var text = lateDevelopers.Developers.Aggregate(shameLateDevelopersRequest.Message, (current, developer) => current + $"\n• <@{developer}>");

                _remindDeveloper.Execute(new RemindDeveloperRequest()
                {
                    Text = text,
                    Channel = shameLateDevelopersRequest.Channel
                });
            }
        }
    }
}
