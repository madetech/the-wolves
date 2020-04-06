using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public interface IGetLateBillablePeople
    {
        GetLateBillablePeopleResponse Execute();
    }
}
