using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindUser
    {
        private readonly IMessageSender _slackGateway;

        public RemindUser(IMessageSender slackGateway)
        {
            _slackGateway = slackGateway;
        }

        public void Execute(RemindUserRequest remindUserRequest)
        {
            _slackGateway.Send(new Message
            {
                Channel = remindUserRequest.Channel,
                Text = remindUserRequest.Text
            });
        }
    }
}