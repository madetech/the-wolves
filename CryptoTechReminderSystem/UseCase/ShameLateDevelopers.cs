using System.Linq;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class ShameLateDevelopers
    {
        private readonly IGetLateDevelopers _getLateDevelopers;
        private readonly ISendReminder _sendReminder;

        public ShameLateDevelopers(IGetLateDevelopers getLateDevelopers, ISendReminder sendReminder)
        {
            _getLateDevelopers = getLateDevelopers;
            _sendReminder = sendReminder;
        }

        public void Execute(ShameLateDevelopersRequest shameLateDevelopersRequest)
        {
            var lateDevelopers = _getLateDevelopers.Execute();
            string text;

            if (!lateDevelopers.Developers.Any())
            {
                text = shameLateDevelopersRequest.NoShameMessage;
            } else
            {
                text = lateDevelopers.Developers.Aggregate(
                    shameLateDevelopersRequest.ShameMessage, 
                    (current, developer) => current + $"\n• <@{developer.Id}>"
                );
            }

            _sendReminder.Execute(new SendReminderRequest
            {
                Text = text,
                Channel = shameLateDevelopersRequest.Channel
            });
        }
    }
}
