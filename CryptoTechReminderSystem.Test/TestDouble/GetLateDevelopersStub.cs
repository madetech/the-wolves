using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetLateBillablePeopleStub : IGetLateBillablePeople
    {
        private readonly List<GetLateBillablePeopleResponse.LateBillablePerson> _lateBillablePeople;

        private static List<GetLateBillablePeopleResponse.LateBillablePerson> DefaultList => new List<GetLateBillablePeopleResponse.LateBillablePerson>
        {
            new GetLateBillablePeopleResponse.LateBillablePerson
                {
                    Id = "W0123CHAN"
                },
            new GetLateBillablePeopleResponse.LateBillablePerson
                {
                    Id = "W123AMON"
                },
            new GetLateBillablePeopleResponse.LateBillablePerson
                {
                    Id = "W789ROSS"
                }
        };
        
        public GetLateBillablePeopleStub(List<string> lateBillablePeople = null)
        {
            var templateBillablePerson = new List<GetLateBillablePeopleResponse.LateBillablePerson>();

            if (lateBillablePeople != null)
                foreach (var billablePerson in lateBillablePeople)
                {
                    templateBillablePerson.Add(new GetLateBillablePeopleResponse.LateBillablePerson
                    {
                        Id = billablePerson
                    });
                }

            _lateBillablePeople = templateBillablePerson.Count == 0 ? DefaultList : templateBillablePerson;
        }
        
        public GetLateBillablePeopleResponse Execute()
        {
            return new GetLateBillablePeopleResponse
            {
                BillablePeople = _lateBillablePeople
            };
        }
    }
}
