using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetProjectManagersWithOpenTimeEntries : IGetLateBillablePeople
    {
        public GetProjectManagersWithOpenTimeEntries(ISlackBillablePersonRetriever slackBillablePersonRetriever, IHarvestBillablePersonRetriever harvestBillablePersonRetriever, ITimeSheetRetriever harvestTimeSheetRetriever, IClock clock) {

        }
        
        public GetLateBillablePeopleResponse Execute() {
            return new GetLateBillablePeopleResponse();
        }
    }
}
