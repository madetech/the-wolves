using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindDeveloper
    {
        private readonly IMessageSender _slackGateway;

        public RemindDeveloper(IMessageSender slackGateway)
        {
            _slackGateway = slackGateway;
        }

        public void Execute(RemindDeveloperRequest remindDeveloperRequest)
        {
            _slackGateway.Send(new Message
            {
                Channel = remindDeveloperRequest.Channel,
                Text = remindDeveloperRequest.Text
            });
        }
    }
}