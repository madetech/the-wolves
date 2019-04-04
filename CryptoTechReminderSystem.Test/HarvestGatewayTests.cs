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

        [SetUp]
        public void Setup()
        {
            _fluentSimulator = new FluentSimulator("http://localhost:8050/");
            _fluentSimulator.Start();
            _harvestGateway = new HarvestGateway("http://localhost:8050/", "xxxx-xxxxxxxxx-xxxx");
        }
        
        [TearDown]
        public void TearDown()
        {
            _fluentSimulator.Stop();
        }
        
        public class User
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
        }

        public void SetUpUsersApiEndpoint(string id, string first_name, string last_name, string email)
        {
            var json = $"{{  \"users\":[    {{      \"id\":{id},      \"first_name\":\"{first_name}\",      \"last_name\":\"{last_name}\",      \"email\":\"{email}\"    }}  ]}}";
            _fluentSimulator.Get("/api/v2/users").Responds(json);
        }
       
        
        [Test]
        public void CanGetDeveloper()
        {
            SetUpUsersApiEndpoint(
                "123",
                "Ting",
                "Ting",
                "ting@email.com"
            );
            
            var response = _harvestGateway.Retrieve();
            
            response.First().FirstName.Should().Be("Ting");
        }
        
        [Test]
        public void CanGetDeveloper2()
        {
            SetUpUsersApiEndpoint(
                "567",
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
            
            _fluentSimulator.ReceivedRequests.First().Headers["Authorization"].Should().Be("Bearer xxxx-xxxxxxxxx-xxxx");
        }

        [Test]
        public void CanGetDeveloperObject()
        { 
            SetUpUsersApiEndpoint(
                "902",
                "Bob",
                "Tings",
                "bobtings@email.com"
            );

            var response = _harvestGateway.Retrieve();

            response.First().FirstName.Should().Be("Bob");

        }
    }
}