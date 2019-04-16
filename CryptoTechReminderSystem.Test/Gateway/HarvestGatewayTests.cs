using System;
using System.IO;
using System.Linq;
using CryptoTechReminderSystem.Gateway;
using FluentAssertions;
using FluentSim;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.Gateway
{
    [TestFixture]
    public class HarvestGatewayTests
    {
        private const string Address = "http://localhost:8050/";
        private const string Token = "xxxx-xxxxxxxxx-xxxx";

        [TestFixture]
        public class CanRequestDevelopers
        {
            private FluentSimulator _harvestApi;
            private HarvestGateway _harvestGateway;
            
            [SetUp]
            public void Setup()
            {
                _harvestApi = new FluentSimulator(Address);
                _harvestApi.Start();
                _harvestGateway = new HarvestGateway(Address, Token);
            }

            [TearDown]
            public void TearDown()
            {
                _harvestApi.Stop();
            }

            private void SetUpUsersApiEndpoint(string id, string firstName, string lastName, string email)
            {
                var json = $"{{  \"users\":[    {{      \"id\":{id},      \"first_name\":\"{firstName}\"" +
                           $",      \"last_name\":\"{lastName}\",      \"email\":\"{email}\"    }}  ]}}";
                _harvestApi.Get("/api/v2/users").Responds(json);
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
                
                _harvestApi.ReceivedRequests.First().Headers["Authorization"].Should().Be("Bearer " + Token);
            }
        }

        [TestFixture]
        public class CanRequestTimeSheets
        {
            private FluentSimulator _harvestApi;
            private HarvestGateway _harvestGateway;
            
            [SetUp]
            public void Setup()
            {
                _harvestApi = new FluentSimulator(Address);
                _harvestGateway = new HarvestGateway(Address, Token);
            }

            [TearDown]
            public void TearDown()
            {
                _harvestApi.Stop();
            }
            
            private void SetUpTimeSheetApiEndpoint(string jsonFilePath, string dateFrom, string dateTo)
            {
                var json = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        jsonFilePath
                    )
                );
                var address = "/api/v2/time_entries";
                _harvestApi.Get(address).WithParameter("from", dateFrom).WithParameter("to", dateTo).Responds(json);
                _harvestApi.Start();
            }

            [Test]
            public void CanGetATimeSheet()
            {
                SetUpTimeSheetApiEndpoint("../../../Gateway/HarvestTimeEntriesApiEndpoint.json", "2019-04-08", "2019-04-12");

                var dateFrom = new DateTimeOffset(
                    new DateTime(2019, 04, 08)
                );
                
                var dateTo = new DateTimeOffset(
                    new DateTime(2019, 04, 12)
                );
                
                var response = _harvestGateway.RetrieveTimeSheets(dateFrom, dateTo);

                response.First().UserId.Should().Be(1782975);
            }

            [Test]
            public void CanGetAnotherTimeSheet()
            {
                SetUpTimeSheetApiEndpoint("../../../Gateway/HarvestTimeEntriesApiEndpoint.json", "2019-04-08", "2019-04-12");

                var dateFrom = new DateTimeOffset(
                    new DateTime(2019, 04, 08)
                );
                
                var dateTo = new DateTimeOffset(
                    new DateTime(2019, 04, 12)
                );
                
                var response = _harvestGateway.RetrieveTimeSheets(dateFrom, dateTo);

                response.Any(entry => entry.UserId == 1782974).Should().Be(true);
            }

            [Test]
            public void CanGetAllTimeSheetProperties()
            {
                SetUpTimeSheetApiEndpoint("../../../Gateway/HarvestTimeEntriesApiEndpoint.json", "2019-04-08", "2019-04-12");

                var dateFrom = new DateTimeOffset(
                    new DateTime(2019, 04, 08)
                );
                
                var dateTo = new DateTimeOffset(
                    new DateTime(2019, 04, 12)
                );
                
                var response = _harvestGateway.RetrieveTimeSheets(dateFrom, dateTo).First();
                response.Id.Should().Be(456709345);
                response.UserId.Should().Be(1782975);
                response.Hours.Should().Be(8.0);
            }
            
            [Test]
            public void CanRequestTimesheetsUsingAStartingAndEndingDate()
            {
                SetUpTimeSheetApiEndpoint("../../../Gateway/HarvestTimeEntriesApiEndpoint.json", "2019-04-08", "2019-04-12");

                var dateFrom = new DateTimeOffset(
                    new DateTime(2019, 04, 08)
                );
                
                var dateTo = new DateTimeOffset(
                    new DateTime(2019, 04, 12)
                );
                
                _harvestGateway.RetrieveTimeSheets(dateFrom, dateTo);

                _harvestApi.ReceivedRequests.First().Url.Should().Be(Address + "api/v2/time_entries?from=2019-04-08&to=2019-04-12");
            }
        }
    }
}

