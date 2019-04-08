using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindLateDevelopers
    {
        public RemindLateDevelopers(GetLateDevelopers getLateDevelopers, RemindDeveloper remindDeveloper, IClock clock)
        {
        }

        public void Execute(RemindLateDevelopersRequest remindLateDevelopersRequest)
        {
        }
    }
}