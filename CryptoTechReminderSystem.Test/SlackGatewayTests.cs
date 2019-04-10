using System;
using System.IO;
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
        private const string Address = "http://localhost:8011/";
        private const string Token = "xxxx-xxxxxxxxx-xxxx";
        private const string PostMessageApiPath = "api/chat.postMessage";
        private const string PostMessageApiUrl = Address + PostMessageApiPath;
        private FluentSimulator _fluentSimulator;
        private SlackGateway _slackGateway;

        public class SlackPostMessageResponse
        {
            [JsonProperty("ok")]
            public bool IsOk;
        }

        [SetUp]
        public void Setup()
        {
            _fluentSimulator = new FluentSimulator(Address);
            _slackGateway = new SlackGateway(Address, Token);
            
            var slackPostMessageResponse = new SlackPostMessageResponse
            {
                IsOk = true
            };
            
            _fluentSimulator.Post(PostMessageApiPath).Responds(slackPostMessageResponse);

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
            _slackGateway.Send(new Message());
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
                
            receivedRequest.Url.Should().Be(PostMessageApiUrl);
        }
        
        [Test]
        public void CanSendAPostMessageRequestWithAToken()
        {
            _slackGateway.Send(new Message());
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
                
            receivedRequest.Headers["Authorization"].Should().Be(
                "Bearer " + Token
            );
        }
        
        [Test]
        public void CanSendAMessageToAUser()
        {
            var channel = "U98DL811";
            var message = new Message()
            {
                Channel = channel
            };
            
            _slackGateway.Send(message);
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
            
            DeserializeObject<PostMessageRequest>(receivedRequest.RequestBody).Channel.Should().Be(channel);

        }
        
        [Test]
        public void CanSendAMessageToAUserWithAText()
        {
            var text = "Please make sure your timesheet is submitted by 13:30 on Friday.";
            var message = new Message()
            {
                Channel = "U0112WTW",
                Text = text
            };
            
            _slackGateway.Send(message);
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
                
            DeserializeObject<PostMessageRequest>(receivedRequest.RequestBody).Text.Should().Be(text);
        }
        
        [Test]
        public void CanSendAGetUsersRequest()
        {
            var json = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../SlackUsersExampleResponse.json"
                )
            );
            _fluentSimulator.Get("/api/users.list").Responds(json);
            
            _slackGateway.RetrieveDevelopers();
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
                
            receivedRequest.Url.Should().Be(Address + "api/users.list");
        }
        
        [Test]
        public void CanSendAGetUsersRequestWithAToken()
        {
            var json = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../SlackUsersExampleResponse.json"
                )
            );
            _fluentSimulator.Get("/api/users.list").Responds(json);
            
            _slackGateway.RetrieveDevelopers();
            
            var receivedRequest = _fluentSimulator.ReceivedRequests.First();
                
            receivedRequest.Headers["Authorization"].Should().Be(
                "Bearer " + Token
            );
        }
    }
}