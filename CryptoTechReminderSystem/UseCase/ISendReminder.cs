using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public interface ISendReminder
    {
        void Execute(SendReminderRequest sendReminderRequest);
    }
}