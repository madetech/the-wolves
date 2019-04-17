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
            public void CanGetDevelopersWithAuthentication()
            {
                SetUpUsersApiEndpoint("1234567", "Wen Ting", "Wang","wenting@ting.com");

                _harvestGateway.RetrieveDevelopers();
                
                _harvestApi.ReceivedRequests.First().Headers["Authorization"].Should().Be($"Bearer {Token}");
            }

            [Test]
            [TestCase("2345678", "Tingky", "Winky", "tingkywinky@ting.com")]
            [TestCase("3456789", "Tingker", "Bell", "tingkerbell@ting.com")]
            [TestCase("4567891", "Ting", "Tings", "tingtings@ting.com")]
            public void CanGetADeveloper(string id, string firstName, string lastName, string email)
            {
                SetUpUsersApiEndpoint(id, firstName, lastName, email);

                var response = _harvestGateway.RetrieveDevelopers();

                response.First().FirstName.Should().Be(firstName);
            }
        }

        [TestFixture]
        public class CanRequestTimeSheets
        {
            private const string ApiTimeSheetPath = "api/v2/time_entries";
            private FluentSimulator _harvestApi;
            private HarvestGateway _harvestGateway;
            private DateTimeOffset _defaultDateFrom;
            private DateTimeOffset _defaultDateTo;

            [SetUp]
            public void Setup()
            {
                _harvestApi = new FluentSimulator(Address);
                _harvestGateway = new HarvestGateway(Address, Token);
                _defaultDateFrom = new DateTimeOffset(
                    new DateTime(2019, 04, 08)
                );
                _defaultDateTo = new DateTimeOffset(
                    new DateTime(2019, 04, 12)
                );
            }

            [TearDown]
            public void TearDown()
            {
                _harvestApi.Stop();
            }
            
            private void SetUpTimeSheetApiEndpoint(string dateFrom, string dateTo)
            {
                var json = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "../../../Gateway/HarvestTimeEntriesApiEndpoint.json"
                    )
                );

                _harvestApi.Get($"/{ApiTimeSheetPath}")
                    .WithParameter("from", dateFrom)
                    .WithParameter("to", dateTo)
                    .Responds(json);
                
                _harvestApi.Start();
            }
            
            [Test]
            [TestCase("08", "12")]
            [TestCase("14", "19")]
            [TestCase("01", "05")]
            public void CanRequestTimeSheetsUsingAStartingAndEndingDate(string dayFrom, string dayTo)
            {
                SetUpTimeSheetApiEndpoint($"2019-04-{dayFrom}", $"2019-04-{dayTo}");

                var dateFrom = new DateTimeOffset(
                    new DateTime(2019, 04, int.Parse(dayFrom))
                );
               
                var dateTo = new DateTimeOffset(
                    new DateTime(2019, 04, int.Parse(dayTo))
                );
                
                _harvestGateway.RetrieveTimeSheets(dateFrom, dateTo);

                _harvestApi.ReceivedRequests.First().Url.Should().Be(
                    $"{Address}{ApiTimeSheetPath}?from=2019-04-{dayFrom}&to=2019-04-{dayTo}"
                );
            }

            [Test]
            [TestCase(1782975)]
            [TestCase(1782974)]
            public void CanGetATimeSheet(int expectedUserId)
            {
                SetUpTimeSheetApiEndpoint("2019-04-08", "2019-04-12");

                var response = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);

                response.Any(entry => entry.UserId == expectedUserId).Should().BeTrue();
            }

            [Test]
            public void CanGetAllTimeSheetProperties()
            {
                SetUpTimeSheetApiEndpoint("2019-04-08", "2019-04-12");

                var response = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);
                
                response.First().Id.Should().Be(456709345);
                response.First().UserId.Should().Be(1782975);
                response.First().Hours.Should().Be(8.0);
            }
        }
    }
}

