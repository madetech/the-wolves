using System;
using System.IO;
using NUnit.Framework;
using FluentAssertions;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentSim;
using Newtonsoft.Json;

namespace CryptoTechReminderSystem.AcceptanceTest
{
    public class CryptoTechReminderSystemTests
    {
        private FluentSimulator _slackApi;
        private FluentSimulator _harvestApi;
        private IMessageSenderAndRetriever _slackGateway;
        private ITimesheetAndDeveloperRetriever _harvestGateway;
        private RemindDeveloper _remindDeveloper;

        private class ClockStub : IClock
        {
            private readonly DateTimeOffset _currentDateTime;

            public ClockStub(DateTimeOffset dateTime)
            {
                _currentDateTime = dateTime;
            }

            public DateTimeOffset Now()
            {
                return _currentDateTime;
            }
        }

        [SetUp]
        public void Setup()
        {
            _slackApi = new FluentSimulator("http://localhost:8009/");
            _slackGateway = new SlackGateway("http://localhost:8009/","xxxx-xxxxxxxxx-xxxx");
            _harvestApi = new FluentSimulator("http://localhost:8010/");
            _harvestGateway = new HarvestGateway("http://localhost:8010/", "xxxx-xxxxxxxxx-xxxx");
            _remindDeveloper = new RemindDeveloper(_slackGateway);
            _slackApi.Start();
            _harvestApi.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _slackApi.Stop();
            _harvestApi.Stop();
        }

        [Ignore("WIP")]
        public void CanRemindLateDevelopersAtTenThirtyOnFriday()
        {            
            var slackGetUsersResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../SlackUsersExampleResponse.json"
                )
            );
            
            _slackApi.Get("/api/users.list").Responds(slackGetUsersResponse);

            _slackApi.Post("/api/chat.postMessage").Responds("\"ok\": true");
            
            var harvestGetUsersResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../HarvestUsersExampleResponse.json"
                )
            );
            
            _harvestApi.Get("/api/v2/users").Responds(harvestGetUsersResponse);
            
            var harvestGetTimeEntriesResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../HarvestTimeEntriesExampleResponse.json"
                )
            );
            
            _harvestApi.Get("/api/v2/time_entries").Responds(harvestGetTimeEntriesResponse);
            
            var getLateDevelopers = new GetLateDevelopers(_slackGateway, _harvestGateway);
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopers, _remindDeveloper, clock);

            remindLateDevelopers.Execute(new RemindLateDevelopersRequest
                {
                    Message = "Please make sure your timesheet is submitted by 13:30 on Friday."
                }
            );

            _slackApi.ReceivedRequests.Count.Should().Be(3);
        }
    }
}