using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public interface IRemindDeveloper
    {
        void Execute(RemindDeveloperRequest remindDeveloperRequest);
    }
}