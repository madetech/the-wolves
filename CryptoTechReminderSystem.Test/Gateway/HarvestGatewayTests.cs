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
        private const string HarvestAccountId = "123456";
        private const string UserAgent = "The Wolves";
        private const string BillablePersonRoles =
            @"Software Engineer, Senior Software Engineer, Senior Engineer, Lead Engineer, 
            Delivery Manager, SRE, Consultant, Delivery Principal";

        private static FluentSimulator _harvestApi;
        private static HarvestGateway _harvestGateway;

        [TestFixture]
        public class CanConstructInstance
        {
            [Test]
            public void CanUseNullRoles()
            {
                string roles = null;

                HarvestGateway gateway = new HarvestGateway(Address, Token, HarvestAccountId, UserAgent, roles);
                gateway.Should().NotBeNull();
            }

            [Test]
            public void ThrowsExceptionWhenAddressNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(()=> new HarvestGateway(null, Token, HarvestAccountId, UserAgent, BillablePersonRoles));
                exception.ParamName.Should().Be("address");
            }

            [Test]
            public void ThrowsExceptionWhenTokenNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(()=> new HarvestGateway(Address, null, HarvestAccountId, UserAgent, BillablePersonRoles));
                exception.ParamName.Should().Be("token");
            }

            [Test]
            public void ThrowsExceptionWhenAccountIdNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new HarvestGateway(Address, Token, null, UserAgent, BillablePersonRoles));
                exception.ParamName.Should().Be("accountId");
            }
        }

        [TestFixture]
        public class CanRequestBillablePeople
        {
            private const string ApiUsersPath = "/api/v2/users";

            [SetUp]
            public void Setup()
            {
                _harvestApi = new FluentSimulator(Address);
                _harvestGateway = new HarvestGateway(Address, Token, HarvestAccountId, UserAgent, BillablePersonRoles);
            }

            [TearDown]
            public void TearDown()
            {
                _harvestApi.Stop();
            }

            [Test]
            public void CanSendOneRequestAtATime()
            {
                SetUpUsersEndpointWithSinglePage();

                _harvestGateway.RetrieveBillablePeople();

                _harvestApi.ReceivedRequests.Count.Should().Be(1);
            }

            [Test]
            [TestCase("Authorization", "Bearer " + Token)]
            [TestCase("Harvest-Account-Id", HarvestAccountId)]
            [TestCase("User-Agent", UserAgent)]
            public void CanGetBillablePeopleWithHeaders(string header, string expected)
            {
                SetUpUsersEndpointWithSinglePage();

                _harvestGateway.RetrieveBillablePeople();

                _harvestApi.ReceivedRequests.First().Headers[header].Should().Be(expected);
            }

            [Test]
            public void CanOnlyGetActiveBillablePeople()
            {
                SetUpUsersEndpointWithSinglePage();

                var response = _harvestGateway.RetrieveBillablePeople();

                response.First().FirstName.Should().Be("Dick");
                response.Count.Should().Be(7);
            }

            [Test]
            [TestCase("Alfred", 35)]
            [TestCase("Harvey", 28)]
            public void CanGetWeeklyHoursForBillablePeople(string name, int hours)
            {
                SetUpUsersEndpointWithSinglePage();

                var response = _harvestGateway.RetrieveBillablePeople();
                response.First(billablePerson => billablePerson.FirstName == name).WeeklyHours.Should().Be(hours);
            }

            [Test]
            public void CanGetBillablePeopleWithPagination()
            {
                SetUpUsersEndpointWithTwoPages();

                var response = _harvestGateway.RetrieveBillablePeople();

                response.Should().HaveCount(3);
            }

            [Test]
            public void CanCacheBillablePeople()
            {
                SetUpUsersEndpointWithSinglePage();

                _harvestGateway.RetrieveBillablePeople();
                _harvestGateway.RetrieveBillablePeople();

                _harvestApi.ReceivedRequests.Should().HaveCount(1);
            }

            private static void SetUpUsersEndpointWithSinglePage()
            {
                var json = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Gateway/ApiEndpointResponse/HarvestUsersResponse.json"
                    )
                );

                _harvestApi.Get(ApiUsersPath)
                    .WithParameter("page", "1")
                    .WithParameter("per_page", "100")
                    .Responds(json);
                _harvestApi.Start();
            }
            private static void SetUpUsersEndpointWithTwoPages()
            {
                var jsonPageOne = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Gateway/ApiEndpointResponse/HarvestUsersResponsePageOne.json"
                    )
                );

                _harvestApi.Get(ApiUsersPath)
                    .WithParameter("page", "1")
                    .WithParameter("per_page", "100")
                    .Responds(jsonPageOne);

                var jsonPageTwo = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Gateway/ApiEndpointResponse/HarvestUsersResponsePageTwo.json"
                    )
                );

                _harvestApi.Get(ApiUsersPath)
                    .WithParameter("page", "2")
                    .WithParameter("per_page", "100")
                    .Responds(jsonPageTwo);
                
                _harvestApi.Start();
            }
        }

        [TestFixture]
        public class CanRequestTimeSheets
        {
            private const string ApiTimeSheetPath = "api/v2/time_entries";
            private const string ApiProjectPath = "api/v2/projects";

            private DateTimeOffset _defaultDateFrom;
            private DateTimeOffset _defaultDateTo;

            [SetUp]
            public void Setup()
            {
                _harvestApi = new FluentSimulator(Address);
                _harvestGateway = new HarvestGateway(Address, Token, HarvestAccountId, UserAgent, BillablePersonRoles);
                _defaultDateFrom = new DateTimeOffset(
                    new DateTime(2019, 04, 08)
                );
                _defaultDateTo = new DateTimeOffset(
                    new DateTime(2019, 04, 12)
                );
            }

            private void SetUpTimeSheetApiEndpointWithOnePage(string dateFrom, string dateTo)
            {
                var json = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Gateway/ApiEndpointResponse/HarvestTimeEntriesApiEndpoint.json"
                    )
                );

                _harvestApi.Get($"/{ApiTimeSheetPath}")
                    .WithParameter("from", dateFrom)
                    .WithParameter("to", dateTo)
                    .WithParameter("page", "1")
                    .Responds(json);

                SetUpUserAssignmentsApiEndpointWithOnePage();

                _harvestApi.Start();
            }

            private void SetUpTimeSheetApiEndpointWithTwoPages(string dateFrom, string dateTo)
            {
                var jsonPageOne = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Gateway/ApiEndpointResponse/HarvestTimeEntriesApiEndpointPageOne.json"
                    )
                );

                _harvestApi.Get($"/{ApiTimeSheetPath}")
                    .WithParameter("from", dateFrom)
                    .WithParameter("to", dateTo)
                    .WithParameter("page", "1")
                    .Responds(jsonPageOne);

                var jsonPageTwo = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Gateway/ApiEndpointResponse/HarvestTimeEntriesApiEndpointPageTwo.json"
                    )
                );

                _harvestApi.Get($"/{ApiTimeSheetPath}")
                    .WithParameter("from", dateFrom)
                    .WithParameter("to", dateTo)
                    .WithParameter("page", "2")
                    .Responds(jsonPageTwo);

                SetUpUserAssignmentsApiEndpointWithOnePage();
                
                _harvestApi.Start();
            }

            private void SetUpUserAssignmentsApiEndpointWithOnePage()
            {
                const int TestProjectId = 26670539;
    
                var jsonPageOne = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Gateway/ApiEndpointResponse/HarvestProject1UserAssignmentsResponse.json"
                    )
                );

                _harvestApi.Get($"/{ApiProjectPath}/{TestProjectId}/user_assignments")
                    .WithParameter("page", "1")
                    .Responds(jsonPageOne);
            }

            [TearDown]
            public void TearDown()
            {
                _harvestApi.Stop();
            }

            [Test]
            public void CanSendOneRequestAtATime()
            {
                SetUpTimeSheetApiEndpointWithTwoPages("2019-04-08", "2019-04-12");

                _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);

                _harvestApi.ReceivedRequests.Count.Should().Be(3);
            }

            [Test]
            [TestCase("Authorization", "Bearer " + Token)]
            [TestCase("Harvest-Account-Id", HarvestAccountId)]
            [TestCase("User-Agent", UserAgent)]
            public void CanGetTimeSheetsWithHeaders(string header, string expected)
            {
                SetUpTimeSheetApiEndpointWithTwoPages("2019-04-08", "2019-04-12");

                _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);

                _harvestApi.ReceivedRequests.First().Headers[header].Should().Be(expected);
            }

            [Test]
            [TestCase("08", "12")]
            [TestCase("14", "19")]
            [TestCase("01", "05")]
            public void CanRequestTimeSheetsWithAStartingAndEndingDate(string dayFrom, string dayTo)
            {
                SetUpTimeSheetApiEndpointWithTwoPages($"2019-04-{dayFrom}", $"2019-04-{dayTo}");

                var dateFrom = new DateTimeOffset(
                    new DateTime(2019, 04, int.Parse(dayFrom))
                );

                var dateTo = new DateTimeOffset(
                    new DateTime(2019, 04, int.Parse(dayTo))
                );

                _harvestGateway.RetrieveTimeSheets(dateFrom, dateTo);

                _harvestApi.ReceivedRequests.First().Url.Should().Be(
                    $"{Address}{ApiTimeSheetPath}?from=2019-04-{dayFrom}&to=2019-04-{dayTo}&page=1"
                );
            }

            [Test]
            [TestCase(1782975)]
            [TestCase(1782974)]
            public void CanGetATimeSheet(int expectedUserId)
            {
                SetUpTimeSheetApiEndpointWithOnePage("2019-04-08", "2019-04-12");

                var response = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);

                response.Any(entry => entry.UserId == expectedUserId).Should().BeTrue();
            }

            [Test]
            public void CanGetAllTimeSheetsWithOnePage()
            {
                SetUpTimeSheetApiEndpointWithOnePage("2019-04-08", "2019-04-12");

                var response = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);

                response.Count.Should().Be(4);
            }

            [Test]
            public void CanGetAllTimeSheetsWithTwoPages()
            {
                SetUpTimeSheetApiEndpointWithTwoPages("2019-04-08", "2019-04-12");

                var response = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);

                response.Count.Should().Be(6);
            }

            [Test]
            public void CanGetAllTimeSheetProperties()
            {
                SetUpTimeSheetApiEndpointWithTwoPages("2019-04-08", "2019-04-12");

                var response = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);

                response.First().Id.Should().Be(456709345);
                response.First().UserId.Should().Be(1782975);
                response.First().Hours.Should().Be(7.0);
            }

            [Test]
            public void CanAssociateProjectManagersWithTimeSheets()
            {
                SetUpTimeSheetApiEndpointWithTwoPages("2019-04-08", "2019-04-12");

                var response = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);
                response.First().ProjectManagerIds.First().Should().Be(1782959);
            }

            [Test]
            public void CanCacheTimeSheetResponses()
            {
                SetUpTimeSheetApiEndpointWithOnePage("2019-04-08", "2019-04-12");

                var response1 = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);
                var response2 = _harvestGateway.RetrieveTimeSheets(_defaultDateFrom, _defaultDateTo);
                
                // 2 x requests to (1 x page of time sheets with 1 x project of user_assignments) == 4 without caching
                _harvestApi.ReceivedRequests.Should().HaveCount(2);
            }
        }
    }
}
