using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.UseCase
{
    public interface ISlackGateway
    {
        void Send(Message message);
    }
}