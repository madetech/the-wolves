using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface IMessageSender
    {
        void Send(Message message);
    }
}