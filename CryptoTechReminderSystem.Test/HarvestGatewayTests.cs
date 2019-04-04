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
        private HarvestGateway _harvestGateway;
        private const string _address = "http://localhost:8050/";
        private const string _token = "xxxx-xxxxxxxxx-xxxx";

        [SetUp]
        public void Setup()
        {
            _fluentSimulator = new FluentSimulator(_address);
            _fluentSimulator.Start();
            _harvestGateway = new HarvestGateway(_address, _token);
        }
        
        [TearDown]
        public void TearDown()
        {
            _fluentSimulator.Stop();
        }

        private void SetUpUsersApiEndpoint(string id, string firstName, string lastName, string email)
        {
            var json = $"{{  \"users\":[    {{      \"id\":{id},      \"first_name\":\"{firstName}\",      \"last_name\":\"{lastName}\",      \"email\":\"{email}\"    }}  ]}}";
            _fluentSimulator.Get("/api/v2/users").Responds(json);
        }
       
        
        [Test]
        public void CanGetDeveloper()
        {
            SetUpUsersApiEndpoint(
                "123",
                "Wen Ting",
                "Wang",
                "ting@email.com"
            );
            
            var response = _harvestGateway.Retrieve();
            
            response.First().FirstName.Should().Be("Wen Ting");
        }
        
        [Test]
        public void CanGetDeveloper2()
        {
            SetUpUsersApiEndpoint(
                "456",
                "Tingker",
                "Bell",
                "tingkerbell@email.com"
            );
            
            var response = _harvestGateway.Retrieve();

            response.First().FirstName.Should().Be("Tingker");
        }
        
        [Test]
        public void CanGetDeveloperWithAuthentication()
        {
            SetUpUsersApiEndpoint(
                "789",
                "Tingky",
                "Winky",
                "tingkywinky@email.com"
            );
            
            var response = _harvestGateway.Retrieve();

            response.First().FirstName.Should().Be("Tingky");
            
            _fluentSimulator.ReceivedRequests.First().Headers["Authorization"].Should().Be("Bearer " + _token);
        }

        [Test]
        public void CanGetDeveloperObject()
        { 
            SetUpUsersApiEndpoint(
                "101",
                "Ting",
                "Tings",
                "tingtings@email.com"
            );

            var response = _harvestGateway.Retrieve();

            response.First().FirstName.Should().Be("Ting");

        }
    }
}