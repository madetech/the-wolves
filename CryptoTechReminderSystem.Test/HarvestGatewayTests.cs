using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using CryptoTechReminderSystem.AcceptanceTest;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using FluentSim;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using static Newtonsoft.Json.JsonConvert;
using HarvestGetUsersResponse = CryptoTechReminderSystem.AcceptanceTest;

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
        public void CanGetDeveloper()
        {
            _fluentSimulator.Get("/api/v2/users").Responds(new Developer
            {
                FirstName = "User"
            });
            var harvestGateway = new HarvestGateway("http://localhost:8050/", "token");
            var response = harvestGateway.Retrieve();
            response.FirstName.Should().Be("User");
        }
        
        [Test]
        public void CanGetDeveloper2()
        {
            _fluentSimulator.Get("/api/v2/users").Responds(new Developer
            {
                FirstName = "User1"
            });
            var harvestGateway = new HarvestGateway("http://localhost:8050/", "token");
            
            var response = harvestGateway.Retrieve();
            response.FirstName.Should().Be("User1");
        }
        
        [Test]
        public void CanGetDeveloperWithAuthentication()
        {
            _fluentSimulator.Get("/api/v2/users").Responds(new Developer
            {
                FirstName = "User2"
            });
            
            var harvestGateway = new HarvestGateway("http://localhost:8050/", "xxxx-xxxxxxxxx-xxxx");

            var response = harvestGateway.Retrieve();
            response.FirstName.Should().Be("User2");
            
            _fluentSimulator.ReceivedRequests.First().Headers["Authorization"].Should().Be("Bearer xxxx-xxxxxxxxx-xxxx");
        }

        [Test]
        public void CanGetDeveloperObject()
        { 
            var jsonResponse = SerializeObject(new Developer
            {
                FirstName = "User3",
                LastName = "Name3",
                Email = "api@harvest.com",
                Hours = 35,
                Id = 1234567
            });
            
            _fluentSimulator.Get("/api/v2/users").Responds(jsonResponse);

            var harvestGateway = new HarvestGateway("http://localhost:8050/", "xxxx-xxxxxxxxx-xxxx");  
            
            var expected = new Developer
            {
                FirstName = "User3",
                LastName = "Name3",
                Email = "api@harvest.com",
                Hours = 35,
                Id = 1234567
            };

            harvestGateway.Retrieve().Should().BeEquivalentTo(expected);
        }
    }
}