using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Gateway
{
    public class MessageSender : IMessageSender
    {
        public MessageSender(string httpLocalhost)
        {
        }

        public void Send(Message message)
        {
        }
    }
}