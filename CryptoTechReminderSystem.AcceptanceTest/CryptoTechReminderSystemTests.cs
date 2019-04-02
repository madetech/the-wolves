using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using System.Net.Sockets;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentSim;
using Newtonsoft.Json;
using static CryptoTechReminderSystem.Gateway.MessageSender;
using static Newtonsoft.Json.JsonConvert;

namespace CryptoTechReminderSystem.AcceptanceTest
{
    public class CryptoTechReminderSystemTests
    {
        private FluentSimulator _fluentSimulator;
        private MessageSender _messageSender;
        private RemindUser _remindUser;
        
        public class SlackPostMessageResponse
        {
            [JsonProperty("ok")] public bool IsOk;
        }

        private static PostMessageRequest GetRequest(ReceivedRequest receivedRequest)
        {
            return DeserializeObject<PostMessageRequest>(receivedRequest.RequestBody);
        }

        private void GivenSlackRespondsWithOk()
        {
            var slackPostMessageResponse = new SlackPostMessageResponse
            {
                IsOk = true
            };

            _fluentSimulator.Post("/api/chat.postMessage").Responds(slackPostMessageResponse);
        }

        private void WhenWeRemindUser(string userId)
        {
            _remindUser.Execute(new RemindUserRequest
            {
                UserId = userId
            });
        }

        private void ThenMessageHasBeenPostedToSlack(string userId)
        {
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();

            receivedRequest.Url.Should().Be(
                "http://localhost:8009/api/chat.postMessage"
            );
            receivedRequest.Headers["Authorization"].Should().Be(
                "Bearer xxxx-xxxxxxxxx-xxxx"
            );
            GetRequest(receivedRequest).channel.Should().Be(userId);
            GetRequest(receivedRequest).text.Should().Be(
                "Please make sure your timesheet is submitted by 13:30 on Friday."
            );
        }

        [SetUp]
        public void Setup()
        {
            _fluentSimulator = new FluentSimulator(
                "http://localhost:8009/"
            );
            
            _fluentSimulator.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _fluentSimulator.Stop();
        }

        [Test]
        public void CanRemindAUser()
        {
            _messageSender = new MessageSender(
                "http://localhost:8009/",
                "xxxx-xxxxxxxxx-xxxx"
            );
            _remindUser = new RemindUser(_messageSender);
            
            GivenSlackRespondsWithOk();

            WhenWeRemindUser("U172L982");

            ThenMessageHasBeenPostedToSlack("U172L982");
        }

        [Test]
        public void CanGetUsersFromHarvest()
        {
            var harvestGetUsersResponse = new HarvestGetUsersResponse
            {
                Success = true
            };

            var getUsers = new GetUsers(new HarvestGateway(
                "http://localhost:8009/",
                "xxxx-xxxxxxxxx-xxxx"
            ));
            var response = getUsers.Execute();
            
            _fluentSimulator.Get("/api/v2/users").Responds(harvestGetUsersResponse);
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();

            receivedRequest.Url.Should().Be(
                "http://localhost:8009/api/v2/users"
            );
            receivedRequest.Headers["Authorization"].Should().Be(
                "Bearer xxxx-xxxxxxxxx-xxxx"
            );

            DeserializeObject<HarvestGetUsersResponse>(receivedRequest.RequestBody).Success.Should().Be(true);
        }
    }

    public class HarvestGetUsersResponse
    {
        public bool Success { get; set; }
    }
}