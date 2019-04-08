using System;
using System.IO;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using FluentSim;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class HarvestGatewayTests
    {
        private const string Address = "http://localhost:8050/";
        private const string Token = "xxxx-xxxxxxxxx-xxxx";
        private FluentSimulator _fluentSimulator;
        private HarvestGateway _harvestGateway;

        [SetUp]
        public void Setup()
        {
            _fluentSimulator = new FluentSimulator(Address);
            _fluentSimulator.Start();
            _harvestGateway = new HarvestGateway(Address, Token);
        }
        
        [TearDown]
        public void TearDown()
        {
            _fluentSimulator.Stop();
        }

        private void SetUpUsersApiEndpoint(string id, string firstName, string lastName, string email)
        {
            var json = $"{{  \"users\":[    {{      \"id\":{id},      \"first_name\":\"{firstName}\"" +
                       $",      \"last_name\":\"{lastName}\",      \"email\":\"{email}\"    }}  ]}}";
            _fluentSimulator.Get("/api/v2/users").Responds(json);
        }
        
        private void SetUpUsersTimeSheetApiEndpoint(string jsonFilePath)
        {
             var json = File.ReadAllText(
                 Path.Combine(
                     AppDomain.CurrentDomain.BaseDirectory,
                     jsonFilePath
                 )
             );
            _fluentSimulator.Get("/api/v2/time_entries").Responds(json);
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
            
            var response = _harvestGateway.RetrieveDevelopers();
            
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
            
            var response = _harvestGateway.RetrieveDevelopers();

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
            
            var response = _harvestGateway.RetrieveDevelopers();

            _fluentSimulator.ReceivedRequests.First().Headers["Authorization"].Should().Be("Bearer " + Token);
        }
        
        [Test]
        public void CanGetATimeSheet()
        {
            SetUpUsersTimeSheetApiEndpoint("../../../HarvestTimeEntriesApiEndpoint.json");
            
            var response = _harvestGateway.RetrieveTimeSheets();
            
            response.First().User.Name.Should().Be("Bob Incomplete");
        }
        
        [Test]
        public void CanGetAnotherTimeSheet()
        {
            SetUpUsersTimeSheetApiEndpoint("../../../HarvestTimeEntriesApiEndpoint.json");
            
            var response = _harvestGateway.RetrieveTimeSheets();

            response.Any(entry => entry.User.Name == "Bruce Wayne").Should().Be(true);
        }
        
        [Test]
        public void CanGetAllTimeSheetProperties()
        {
            SetUpUsersTimeSheetApiEndpoint("../../../HarvestTimeEntriesApiEndpoint.json");
            
            var response = _harvestGateway.RetrieveTimeSheets().First();
            response.Id.Should().Be(456709345);
            response.User.Name.Should().Be("Bob Incomplete");
            response.User.Id.Should().Be(1782975);
            response.Hours.Should().Be(8.0);
            response.TimeSheetDate.Should().Be("2019-02-25");
            response.CreatedAt.Should().BeSameDateAs(new DateTime(2019, 03,01, 10, 10, 00));
            response.UpdatedAt.Should().BeSameDateAs(new DateTime(2019, 03,01, 10, 10, 00));
        }
    }
}