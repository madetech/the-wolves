using System;
using System.IO;
using System.Linq;
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

        [TestFixture]
        public class CanRequestDevelopers
        {
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

                _harvestGateway.RetrieveDevelopers();
                
                _fluentSimulator.ReceivedRequests.First().Headers["Authorization"].Should().Be("Bearer " + Token);
            }
        }

        [TestFixture]
        public class CanRequestTimeSheets
        {
            private FluentSimulator _fluentSimulator;
            private HarvestGateway _harvestGateway;
            
            [SetUp]
            public void Setup()
            {
                _fluentSimulator = new FluentSimulator(Address);
                _fluentSimulator.Start();
                _harvestGateway = new HarvestGateway(Address, Token);
                SetUpTimeSheetApiEndpoint("../../../HarvestTimeEntriesApiEndpoint.json");
            }

            [TearDown]
            public void TearDown()
            {
                _fluentSimulator.Stop();
            }
            
            private void SetUpTimeSheetApiEndpoint(string jsonFilePath)
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
            public void CanGetATimeSheet()
            {
                var response = _harvestGateway.RetrieveTimeSheets();

                response.First().UserId.Should().Be(1782975);
            }

            [Test]
            public void CanGetAnotherTimeSheet()
            {
                var response = _harvestGateway.RetrieveTimeSheets();

                response.Any(entry => entry.UserId == 1782974).Should().Be(true);
            }

            [Test]
            public void CanGetAllTimeSheetProperties()
            {
                var response = _harvestGateway.RetrieveTimeSheets().First();
                response.Id.Should().Be(456709345);
                response.UserId.Should().Be(1782975);
                response.Hours.Should().Be(8.0);
            }
        }
    }
}
