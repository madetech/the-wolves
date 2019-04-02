using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetUsers
    {
        private HarvestGateway _requester;

        public GetUsers(HarvestGateway harvestRequester)
        {
            _requester = harvestRequester;
        }

        public string Execute()
        {
            return "asdfasdf";
        }
    }
}