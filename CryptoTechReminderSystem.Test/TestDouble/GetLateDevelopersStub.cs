using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetLateDevelopersStub : IGetLateDevelopers
    {
        private readonly List<GetLateDevelopersResponse.LateDeveloper> _lateDevelopers;

        private static List<GetLateDevelopersResponse.LateDeveloper> DefaultList => new List<GetLateDevelopersResponse.LateDeveloper>
        {
            new GetLateDevelopersResponse.LateDeveloper
                {
                    Id = "W0123CHAN"
                },
            new GetLateDevelopersResponse.LateDeveloper
                {
                    Id = "W123AMON"
                },
            new GetLateDevelopersResponse.LateDeveloper
                {
                    Id = "W789ROSS"
                }
        };
        
        public GetLateDevelopersStub(List<string> lateDevelopers = null)
        {
            var templateDeveloper = new List<GetLateDevelopersResponse.LateDeveloper>();

            if (lateDevelopers != null)
                foreach (var developer in lateDevelopers)
                {
                    templateDeveloper.Add(new GetLateDevelopersResponse.LateDeveloper
                    {
                        Id = developer
                    });
                }

            _lateDevelopers = templateDeveloper.Count == 0 ? DefaultList : templateDeveloper;
        }
        
        public GetLateDevelopersResponse Execute()
        {
            return new GetLateDevelopersResponse
            {
                Developers = _lateDevelopers
            };
        }
    }
}