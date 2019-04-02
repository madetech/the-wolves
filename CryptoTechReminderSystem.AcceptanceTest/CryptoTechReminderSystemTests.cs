using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentSim;
using Newtonsoft.Json;
using static CryptoTechReminderSystem.Gateway.SlackGateway;
using static Newtonsoft.Json.JsonConvert;

namespace CryptoTechReminderSystem.AcceptanceTest
{
    public class CryptoTechReminderSystemTests
    {
        private FluentSimulator _fluentSimulator;
        private SlackGateway _slackGateway;
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

        private void WhenWeRemindUser(string channel, string text)
        {
            _remindUser.Execute(new RemindUserRequest
            {
                Channel = channel,
                Text = text
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
            GetRequest(receivedRequest).Channel.Should().Be(userId);
            GetRequest(receivedRequest).Text.Should().Be(
                "Please make sure your timesheet is submitted by 13:30 on Friday."
            );
        }

        [SetUp]
        public void Setup()
        {
            _fluentSimulator = new FluentSimulator(
                "http://localhost:8009/"
            );
            _slackGateway = new SlackGateway(
                "http://localhost:8009/",
                "xxxx-xxxxxxxxx-xxxx"
            );
            _remindUser = new RemindUser(_slackGateway);
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

            WhenWeRemindUser("U172L982", "Please make sure your timesheet is submitted by 13:30 on Friday.");

            ThenMessageHasBeenPostedToSlack("U172L982");
        }
    }
}