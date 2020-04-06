using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetLateBillablePeopleEmptyStub : IGetLateBillablePeople
    {
        public GetLateBillablePeopleResponse Execute()
        {
            return new GetLateBillablePeopleResponse
            {
                Developers = new List<GetLateBillablePeopleResponse.LateDeveloper>()
            };
        }
    }
}
