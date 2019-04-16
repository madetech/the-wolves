using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.UseCase
{
    public class SendReminder : ISendReminder
    {
        private readonly IMessageSender _slackGateway;

        public SendReminder(IMessageSender slackGateway)
        {
            _slackGateway = slackGateway;
        }

        public void Execute(SendReminderRequest sendReminderRequest)
        {
            _slackGateway.Send(new Message
            {
                Channel = sendReminderRequest.Channel,
                Text = sendReminderRequest.Text
            });
        }
    }
}