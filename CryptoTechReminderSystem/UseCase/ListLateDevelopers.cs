using System.Linq;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class ListLateDevelopers
    {
        private readonly IGetLateDevelopers _getLateDevelopers;
        private readonly ISendReminder _sendReminder;

        public ListLateDevelopers(IGetLateDevelopers getLateDevelopers, ISendReminder sendReminder)
        {
            _getLateDevelopers = getLateDevelopers;
            _sendReminder = sendReminder;
        }

        public void Execute(ListLateDevelopersRequest listLateDevelopersRequest)
        {
            var lateDevelopers = _getLateDevelopers.Execute();
            string text;

            if (!lateDevelopers.Developers.Any())
            {
                text = listLateDevelopersRequest.NoLateDevelopersMessage;
            } else
            {
                text = lateDevelopers.Developers.Aggregate(
                    listLateDevelopersRequest.LateDevelopersMessage, 
                    (current, developer) => current + $"\n• <@{developer.Id}>"
                );
            }

            _sendReminder.Execute(new SendReminderRequest
            {
                Text = text,
                Channel = listLateDevelopersRequest.Channel
            });
        }
    }
}
