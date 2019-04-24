using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetLateDevelopersSpy : IGetLateDevelopers
    {
        public bool Called { private set; get; }

        public GetLateDevelopersResponse Execute()
        {
            Called = true;
            
            return new GetLateDevelopersResponse
            {
                Developers = new List<GetLateDevelopersResponse.LateDeveloper>
                {
                    new GetLateDevelopersResponse.LateDeveloper()
                    {
                        Id = "U9034950",
                        Email = "UncleCraig@aol.com"
                    }
                }
            };
        }
    }
}