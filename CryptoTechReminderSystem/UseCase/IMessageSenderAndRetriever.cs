using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public interface IMessageSenderAndRetriever: IMessageSender, ISlackDeveloperRetriever
    {
    }
}