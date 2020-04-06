using System.Collections.Generic;

namespace CryptoTechReminderSystem.Boundary
{
    public class GetLateBillablePeopleResponse
    {
        public List<LateDeveloper> BillablePeople;

        public class LateDeveloper
        {
            public string Id { get; set; }
            public string Email { get; set; }
        }
    }
    
}
