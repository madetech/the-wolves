using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindUser
    {
        private readonly ISlackGateway _slackGateway;

        public RemindUser(ISlackGateway slackGateway)
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