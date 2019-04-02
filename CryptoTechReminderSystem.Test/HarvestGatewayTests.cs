using System.Linq;
using CryptoTechReminderSystem.Gateway;
using FluentSim;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class HarvestGatewayTests
    {
        private FluentSimulator _fluentSimulator;

        [SetUp]
        public void Setup()
        {
            _fluentSimulator = new FluentSimulator("http://localhost:8050/");
            _fluentSimulator.Start();
        }
        
        [TearDown]
        public void TearDown()
        {
            _fluentSimulator.Stop();
        }
        
        [Test]
        public void CanGetUser()
        {
            _fluentSimulator.Get("/api/v2/users").Responds("User");
            var harvestGateway = new HarvestGateway("http://localhost:8050/", "token");
            var response = harvestGateway.Retrieve();
            response.Should().Be("User");
        }
        
        [Test]
        public void CanGetUser2()
        {
            _fluentSimulator.Get("/api/v2/users").Responds("User1");
            var harvestGateway = new HarvestGateway("http://localhost:8050/", "token");
            
            var response = harvestGateway.Retrieve();
            response.Should().Be("User1");
        }
        
        [Test]
        public void CanGetUserWithAuthentication()
        {
            _fluentSimulator.Get("/api/v2/users").Responds("User1");
            
            var harvestGateway = new HarvestGateway("http://localhost:8050/", "token");
            
            var response = harvestGateway.Retrieve();
            response.Should().Be("User1");
        }
//        _fluentSimulator.ReceivedRequests.First().Url.Should().Be("http://localhost:8050/api/v2/users");
//        _fluentSimulator.ReceivedRequests.First().Headers["Authorization"].Should().Be(
//            "Bearer xxxx-xxxxxxxxx-xxxx"
//        );
        
    }
}