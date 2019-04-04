using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class GetDevelopersTests
    {
        private class HarvestGatewayStub : IDeveloperRetriever
        {
            public string FirstName { private get; set; }
            public IList<Developer> Retrieve()
            {
            
                IList<Developer> result = new List<Developer>()
                {
                    new Developer()
                    {
                        FirstName = FirstName
                    }
                };

                return result;
            }
        }

        private class HarvestGatewaySpy : IDeveloperRetriever
        {
            public bool IsCalled = false;
            public IList<Developer> Retrieve()
            {
                IsCalled = true;
                IList<Developer> result = new List<Developer>()
                {
                    new Developer()
                };

                return result;
            }
        }
        
        [Test]
        public void CanRequestFromGateway()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var getDevelopers = new GetLateDevelopers(harvestGatewaySpy);
            
            getDevelopers.Execute();
            
            harvestGatewaySpy.IsCalled.Should().BeTrue();
        }
        
        [Test]
        public void CanGetOneDeveloper()
        {
            var harvestGatewayStub = new HarvestGatewayStub
            {
                FirstName = "John"
            };
            var getDevelopers = new GetLateDevelopers(harvestGatewayStub);
            
            var response = getDevelopers.Execute();
            
            response.First().FirstName.Should().Be("John");
        }
        
        [Test]
        public void CanGetAnotherDeveloper()
        {
            var harvestGatewayStub = new HarvestGatewayStub
            {
                FirstName = "Jim"
            };
            var getDevelopers = new GetLateDevelopers(harvestGatewayStub);
            
            var response = getDevelopers.Execute();
            
            response.First().FirstName.Should().Be("Jim");
        }
        
        [Test]
        public void CanGetAListOfDevelopers()
        {
            var harvestGatewayStub = new HarvestGatewayStub
            {
                FirstName = "Jim"
            };
            var getDevelopers = new GetLateDevelopers(harvestGatewayStub);
            
            var response = getDevelopers.Execute();
            
            response.First().FirstName.Should().Be("Jim");
        }
    }
}