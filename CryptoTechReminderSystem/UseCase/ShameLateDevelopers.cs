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
            if (_clock.Now().Hour == 13 && _clock.Now().Minute == 30)
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
