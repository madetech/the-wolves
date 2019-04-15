using System.Linq;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class ShameLateDevelopers
    {
        private IGetLateDevelopers _getLateDevelopers;
        private IRemindDeveloper _remindDeveloper;

        public ShameLateDevelopers(IGetLateDevelopers getLateDevelopers, IRemindDeveloper remindDeveloper, IClock clock)
        {
            _getLateDevelopers = getLateDevelopers;
            _remindDeveloper = remindDeveloper;

        }

        public void Execute(ShameLateDevelopersRequest shameLateDevelopersRequest)
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