using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentSim;
using Newtonsoft.Json;

namespace CryptoTechReminderSystem.AcceptanceTest
{
    public class CryptoTechReminderSystemTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CanRemindAUser()
        {
            var fluentSimulator = new FluentSimulator("http://localhost:8009/");
            var slackGateway = new MessageSender("http://localhost:8009/");
            var remindUser = new RemindUser(slackGateway);
            var remindUserRequest = new RemindUserRequest
            {
                UserId = "U172L982"
            };
            var slackPostMessageResponse = new SlackPostMessageResponse
            {
                IsOk = true
            };

            fluentSimulator.Start();
            fluentSimulator.Post("/api/chat.postMessage").Responds(slackPostMessageResponse);
            
            remindUser.Execute(remindUserRequest);

            fluentSimulator.ReceivedRequests.First().Url.Should().Be("/api/chat.postMessage");
        }
        
    }

    public class SlackPostMessageResponse
    {
        [JsonProperty("ok")]
        public bool IsOk;
    }
}