using System.Collections;
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
        [Test]
        public void CanRequestFromGateway()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var getDevelopers = new GetDevelopers(harvestGatewaySpy);
            getDevelopers.Execute();
            harvestGatewaySpy.IsCalled.Should().BeTrue();
        }
        
        [Test]
        public void CanGetOneDeveloper()
        {
            var harvestGatewayStub = new HarvestGatewayStub();
            harvestGatewayStub.FirstName = "John";
            var getDevelopers = new GetDevelopers(harvestGatewayStub);
            var response = getDevelopers.Execute();
            response.First().FirstName.Should().Be("John");
        }
        
        [Test]
        public void CanGetAnotherDeveloper()
        {
            var harvestGatewayStub = new HarvestGatewayStub();
            harvestGatewayStub.FirstName = "Jim";
            var getDevelopers = new GetDevelopers(harvestGatewayStub);
            var response = getDevelopers.Execute();
            response.First().FirstName.Should().Be("Jim");
        }
        
        [Test]
        public void CanGetAListOfDevelopers()
        {
            var harvestGatewayStub = new HarvestGatewayStub();
            harvestGatewayStub.FirstName = "Jim";
            var getDevelopers = new GetDevelopers(harvestGatewayStub);
            var response = getDevelopers.Execute();
            response.First().FirstName.Should().Be("Jim");
        }
    }

    public class HarvestGatewayStub : IDeveloperRetriever
    {
        public string FirstName { get; set; }
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

    public class HarvestGatewaySpy : IDeveloperRetriever
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
}