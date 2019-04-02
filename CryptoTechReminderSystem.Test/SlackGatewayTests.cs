using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using FluentAssertions;
using FluentSim;
using Newtonsoft.Json;
using NUnit.Framework;
using static Newtonsoft.Json.JsonConvert;

namespace CryptoTechReminderSystem.Test
{
    public class SlackGatewayTests
    {
        private FluentSimulator _fluentSimulator;
        
        public class SlackPostMessageResponse
        {
            [JsonProperty("ok")] public bool IsOk;
        }

        [SetUp]
        public void Setup()
        {
            _fluentSimulator = new FluentSimulator("http://localhost:8011/");
            
            var slackPostMessageResponse = new SlackPostMessageResponse
            {
                IsOk = true
            };
            
            _fluentSimulator.Post("/api/chat.postMessage").Responds(slackPostMessageResponse);

            _fluentSimulator.Start();
        }
        
        [TearDown]
        public void TearDown()
        {
            _fluentSimulator.Stop();
        }
        
        [Test]
        public void CanSendAPostMessageRequest()
        {
            var messageSender = new SlackGateway("http://localhost:8011/", "xxxx-xxxxxxxxx-xxxx");
            
            messageSender.Send(new Message());
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
                
            receivedRequest.Url.Should().Be("http://localhost:8011/api/chat.postMessage");
        }
        
        [Test]
        public void CanSendAPostMessageRequestWithAToken()
        {
            var messageSender = new SlackGateway("http://localhost:8011/", "xxxx-xxxxxxxxx-xxxx");
            
            messageSender.Send(new Message());
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
                
            receivedRequest.Url.Should().Be("http://localhost:8011/api/chat.postMessage");
        }
        
        [Test]
        public void CanSendAMessageToAUser()
        {
            var messageSender = new SlackGateway("http://localhost:8011/", "xxxx-xxxxxxxxx-xxxx");
            var message = new Message()
            {
                Channel = "U98DL811"
            };
            messageSender.Send(message);
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
            
            receivedRequest.Headers["Authorization"].Should().Be(
                "Bearer xxxx-xxxxxxxxx-xxxx"
            );
        }
        
        [Test]
        public void CanSendAMessageToAUserWithAText()
        {
            var messageSender = new SlackGateway("http://localhost:8011/", "xxxx-xxxxxxxxx-xxxx");
            var message = new Message()
            {
                Channel = "U98DL811",
                Text = "Please make sure your timesheet is submitted by 13:30 on Friday."
            };
            messageSender.Send(message);
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
                
            DeserializeObject<PostMessageRequest>(receivedRequest.RequestBody).Text.Should().Be("Please make sure your timesheet is submitted by 13:30 on Friday.");
        }
    }
}