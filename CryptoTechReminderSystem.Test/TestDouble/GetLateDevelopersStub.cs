using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetLateDevelopersStub : IGetLateDevelopers
    {
        private readonly List<string> _lateDevelopers;
        
        public GetLateDevelopersStub(List<string> lateDevelopers = null)
        {
            if (lateDevelopers == null)
            {
                _lateDevelopers = new List<string>
                {
                    "W0123CHAN",
                    "W123AMON",
                    "W789ROSS"
                };
            }
            else
            {
                _lateDevelopers = lateDevelopers;
            }
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