using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetLateBillablePeopleStub : IGetLateBillablePeople
    {
        private readonly List<GetLateBillablePeopleResponse.LateDeveloper> _lateBillablePeople;

        private static List<GetLateBillablePeopleResponse.LateDeveloper> DefaultList => new List<GetLateBillablePeopleResponse.LateDeveloper>
        {
            new GetLateBillablePeopleResponse.LateDeveloper
                {
                    Id = "W0123CHAN"
                },
            new GetLateBillablePeopleResponse.LateDeveloper
                {
                    Id = "W123AMON"
                },
            new GetLateBillablePeopleResponse.LateDeveloper
                {
                    Id = "W789ROSS"
                }
        };
        
        public GetLateBillablePeopleStub(List<string> lateBillablePeople = null)
        {
            var templateDeveloper = new List<GetLateBillablePeopleResponse.LateDeveloper>();

            if (lateBillablePeople != null)
                foreach (var developer in lateBillablePeople)
                {
                    templateDeveloper.Add(new GetLateBillablePeopleResponse.LateDeveloper
                    {
                        Id = developer
                    });
                }

            _lateBillablePeople = templateDeveloper.Count == 0 ? DefaultList : templateDeveloper;
        }
        
        public GetLateBillablePeopleResponse Execute()
        {
            return new GetLateBillablePeopleResponse
            {
                Developers = _lateBillablePeople
            };
        }
    }
}
