using System.Collections.Generic;

namespace CryptoTechReminderSystem.Boundary
{
    public class GetLateBillablePeopleResponse
    {
        public List<LateBillablePerson> BillablePeople;

        public class LateBillablePerson
        {
            public string Id { get; set; }
            public string Email { get; set; }
        }
    }
    
}
