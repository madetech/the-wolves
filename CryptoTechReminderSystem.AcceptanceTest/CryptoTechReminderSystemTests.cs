using NUnit.Framework;
using FluentAssertions;
using System.Linq;
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
            _messageSender = new MessageSender(
                "http://localhost:8009/",
                "xxxx-xxxxxxxxx-xxxx"
            );
            _remindUser = new RemindUser(_messageSender);
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
            GivenSlackRespondsWithOk();

            WhenWeRemindUser("U172L982");

            ThenMessageHasBeenPostedToSlack("U172L982");
        }
    }
}