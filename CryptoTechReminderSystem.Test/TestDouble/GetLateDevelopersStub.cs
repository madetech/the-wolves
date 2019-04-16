using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetLateDevelopersStub : IGetLateDevelopers
    {
        private readonly List<string> _lateDevelopers;

        private static List<string> _defaultList => new List<string>
        {
            "W0123CHAN",
            "W123AMON",
            "W789ROSS"
        };
        
        public GetLateDevelopersStub(List<string> lateDevelopers = null)
        {
            _lateDevelopers = lateDevelopers ?? _defaultList;
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