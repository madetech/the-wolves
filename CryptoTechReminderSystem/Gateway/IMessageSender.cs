using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.UseCase
{
    public interface IMessageSender
    {
        void Send(Message message);
    }
}