using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using CryptoTechReminderSystem.AcceptanceTest;
using CryptoTechReminderSystem.Gateway;
using FluentSim;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using static Newtonsoft.Json.JsonConvert;

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
            
            var harvestGateway = new HarvestGateway("http://localhost:8050/", "xxxx-xxxxxxxxx-xxxx");

            var response = harvestGateway.Retrieve();
            response.Should().Be("User1");
            
            _fluentSimulator.ReceivedRequests.First().Headers["Authorization"].Should().Be("Bearer xxxx-xxxxxxxxx-xxxx");
        }

        [Test]
        public void CanGetAJsonWithUsers()
        { 
            var jsonResponse = JsonConvert.SerializeObject(new HarvestGetUsersResponse()
            {
                Success = true
            });
            
            _fluentSimulator.Get("/api/v2/users").Responds(jsonResponse);

            var harvestGateway = new HarvestGateway("http://localhost:8050/", "xxxx-xxxxxxxxx-xxxx");

            var response = DeserializeObject<HarvestGetUsersResponse>(harvestGateway.Retrieve());

            response.Success.Should().Be(true);
        }
    }
}