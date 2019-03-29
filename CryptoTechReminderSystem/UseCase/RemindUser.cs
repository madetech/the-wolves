using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindUser
    {
        private readonly IMessageSender _messageSender;

        public RemindUser(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public void Execute(RemindUserRequest remindUserRequest)
        {
            _messageSender.Send(new Message
            {
                UserId = remindUserRequest.UserId
            });
        }
    }

    public interface IMessageSender
    {
        void Send(Message message);
    }
}