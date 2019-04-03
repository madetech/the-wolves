using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetUsers
    {
        private HarvestGateway _requester;

        public GetUsers(HarvestGateway harvestRequester)
        {
            _requester = harvestRequester;
            _requester.Retrieve();
        }

        public Developer Execute()
        {
            return new Developer()
            {
                Id = 007,
                FirstName = "TestName",
                LastName = "TestName",
                Email = "email@email.com",
                Hours = 0
            };
        }
    }
}