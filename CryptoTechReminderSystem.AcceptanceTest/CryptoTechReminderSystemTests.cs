using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentSim;
using Newtonsoft.Json.Linq;

namespace CryptoTechReminderSystem.AcceptanceTest
{
    public class CryptoTechReminderSystemTests
    {
        private const string SlackApiAddress = "http://localhost:8009/";
        private const string SlackApiUsersPath = "api/users.list";
        private const string SlackApiPostMessagePath = "api/chat.postMessage";
        private static FluentSimulator _slackApi;
        private static SlackGateway _slackGateway;
        private static SendReminder _sendReminder;

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

        private static void HandleSetUp()
        {
            _slackApi = new FluentSimulator(SlackApiAddress);
            _slackGateway = new SlackGateway(SlackApiAddress,"xxxx-xxxxxxxxx-xxxx");
            _sendReminder = new SendReminder(_slackGateway);
            
            var slackGetUsersResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../ApiEndpointResponse/SlackUsersResponse.json"
                )
            );

            _slackApi.Get("/" + SlackApiUsersPath).Responds(slackGetUsersResponse);

            _slackApi.Post("/" + SlackApiPostMessagePath).Responds(new { ok = true });

            _slackApi.Start();
        }

        [SetUp]
        public void Setup()
        {
            HandleSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            _slackApi.Stop();
        }

        [Test]
        public void CanRemindLateBillablePeopleOnAFriday()
        {                      
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            
            var getLateBillablePeople = new GetLateBillablePeople(_slackGateway, clock);

            var remindLateBillablePeople = new RemindLateBillablePeople(getLateBillablePeople, _sendReminder);

            remindLateBillablePeople.Execute(
                new RemindLateBillablePeopleRequest
                {
                    Message = "Please make sure your timesheet is submitted by 13:30 today."
                }
            );
            
            _slackApi.ReceivedRequests.Count.Should().Be(4);
            
            _slackApi.ReceivedRequests.Should()
                .Contain(request => request.Url.ToString() == SlackApiAddress + SlackApiPostMessagePath);   
        }
        
        [Test]
        public void CanRemindLateBillablePeopleEndOfTheMonth()
        {                      
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 07, 31, 10, 30, 0)
                )
            );
            
            var getLateBillablePeople = new GetLateBillablePeople(_slackGateway, clock);
            
            var remindLateBillablePeople = new RemindLateBillablePeople(getLateBillablePeople, _sendReminder);

            remindLateBillablePeople.Execute(
                new RemindLateBillablePeopleRequest
                {
                    Message = "Please make sure your timesheet is submitted by 13:30 today."
                }
            );
            
            _slackApi.ReceivedRequests.Should()
                .Contain(request => request.RawUrl.ToString() == "/" + SlackApiUsersPath);   
            _slackApi.ReceivedRequests.Count(request => request.RawUrl.ToString() == "/" + SlackApiPostMessagePath)
                .Should().Be(3);
        }
    }
}
