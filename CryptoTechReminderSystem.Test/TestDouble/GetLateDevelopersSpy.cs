using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetLateBillablePeopleSpy : IGetLateBillablePeople
    {
        public bool Called { private set; get; }

        public GetLateBillablePeopleResponse Execute()
        {
            Called = true;
            
            return new GetLateBillablePeopleResponse
            {
                BillablePeople = new List<GetLateBillablePeopleResponse.LateDeveloper>
                {
                    new GetLateBillablePeopleResponse.LateDeveloper()
                    {
                        Id = "U9034950",
                        Email = "UncleCraig@aol.com"
                    }
                }
            };
        }
    }
}
