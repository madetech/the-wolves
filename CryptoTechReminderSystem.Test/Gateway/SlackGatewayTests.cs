using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using FluentAssertions;
using FluentSim;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.Gateway
{
    [TestFixture]
    public class SlackGatewayTests
    {
        private const string Address = "http://localhost:8011/";
        private const string Token = "xxxx-xxxxxxxxx-xxxx";
        private const string PostMessageApiPath = "api/chat.postMessage";
        private const string PostMessageApiUrl = Address + PostMessageApiPath;

        [TestFixture]
        public class CanSendMessage
        {
            private SlackSimulator _slackApi;
            private SlackGateway _slackGateway;
            
            class SlackSimulator
            {
                private FluentSimulator _simulator;
                public bool SpyMethodCalled;
                public string ReceivedErrorMessage;

                public SlackSimulator(string address)
                {
                    _simulator = new FluentSimulator(address);
                    SpyMethodCalled = false;
                }
                
                public ReceivedRequest[] ReceivedRequests()
                {
                    return _simulator.ReceivedRequests.ToArray();
                }

                public void RespondWithOk()
                {
                    _simulator.Post("/" + PostMessageApiPath).Responds(new { ok = true });
                }
                public void RespondWithError()
                {
                    _simulator.Post("/" + PostMessageApiPath).Responds(
                        new {ok = false, error = "error message"});
                }
              
                public void Start()
                {
                    _simulator.Start();
                }

                public void Stop()
                {
                    _simulator.Stop();
                }

                public void SpyMethod()
                {
                    SpyMethodCalled = true;
                }

                public void HandleError(Exception exception)
                {
                    ReceivedErrorMessage = exception.Message;
                }
            }
            
            [SetUp]
            public void Setup()
            {
                _slackApi = new SlackSimulator(Address);
                _slackGateway = new SlackGateway(Address, Token);
                _slackApi.Start();
                _slackApi.RespondWithOk();
            }
            
            [TearDown]
            public void TearDown()
            {
                _slackApi.Stop();
            }
            
            [Test]
            public void CanSendAPostMessageRequest()
            {
                _slackGateway.Send(new Message());
            
                var receivedRequest = _slackApi.ReceivedRequests().First();
                
                receivedRequest.Url.Should().Be(PostMessageApiUrl);
            }
            
            [Test]
            public void CanSendAPostMessageRequestWithAToken()
            {
                _slackGateway.Send(new Message());
            
                var receivedRequest = _slackApi.ReceivedRequests().First();
                
                receivedRequest.Headers["Authorization"].Should().Be("Bearer " + Token);
            }

            [Test]
            public void CanSendAPostMessageRequestWithAChannel()
            {
                var channel = "U98DL811";
                var message = new Message
                {
                    Channel = channel
                };
            
                _slackGateway.Send(message);
            
                var receivedRequest = _slackApi.ReceivedRequests().First();
            
                JObject.Parse(receivedRequest.RequestBody)["channel"].ToString().Should().Be(channel);
            }
        
            [Test]
            public void CanSendAPostMessageRequestWithText()
            {
                var text = "Please make sure your timesheet is submitted by 13:30 on Friday.";
                var message = new Message
                {
                    Text = text
                };
            
                _slackGateway.Send(message);
            
                var receivedRequest = _slackApi.ReceivedRequests().First();
                
                JObject.Parse(receivedRequest.RequestBody)["text"].ToString().Should().Be(text);
            }
            
            [Test]
            public void CanSendAPostMessageRequestWithAChannelAndText()
            {
                var channel = "U98ZL999";
                var text = "Please make sure your timesheet is submitted by 13:30 on Friday.";
                var message = new Message
                {
                    Channel = channel,
                    Text = text
                };
            
                _slackGateway.Send(message);
            
                var receivedRequest = _slackApi.ReceivedRequests().First();
                
                JObject.Parse(receivedRequest.RequestBody)["channel"].ToString().Should().Be(channel);
                JObject.Parse(receivedRequest.RequestBody)["text"].ToString().Should().Be(text);
            }
            
            [Test]
            public void CanSendAPostMessageRequestWithSuccess()
            {
                var channel = "U98ZL999";
                var text = "Please make sure your timesheet is submitted by 13:30 on Friday.";
                var message = new Message
                {
                    Channel = channel,
                    Text = text
                };
                
                var response = _slackGateway.Send(message);

                response.OnSuccess(f => _slackApi.SpyMethod());
                _slackApi.SpyMethodCalled.Should().BeTrue();
                response.OnError(error => _slackApi.HandleError(error));
                _slackApi.ReceivedErrorMessage.Should().BeNullOrEmpty();
            }
            
            [Test]
            public void CanRespondWithErrorIfMessageSendFails()
            {
                _slackApi.RespondWithError();
                var channel = "U98ZL999";
                var text = "Please make sure your timesheet is submitted by 13:30 on Friday.";
                var message = new Message
                {
                    Channel = channel,
                    Text = text
                };
                
                var response = _slackGateway.Send(message);

                response.OnSuccess(f => _slackApi.SpyMethod());
                _slackApi.SpyMethodCalled.Should().BeFalse();
                response.OnError(error => _slackApi.HandleError(error));
                _slackApi.ReceivedErrorMessage.Should().Be("error message");
            }
        }

        [TestFixture]
        public class CanRetrieveDevelopers
        {
            private FluentSimulator _slackApi;
            private SlackGateway _slackGateway;
            private IList<SlackDeveloper> _response;

            [SetUp]
            public void SetUp()
            {
                _slackApi = new FluentSimulator(Address);
                _slackGateway = new SlackGateway(Address, Token);
            
                var json = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "../../../Gateway/ApiEndpointResponse/SlackUsersResponse.json"
                    )
                );
                
                _slackApi.Get("/api/users.list").Responds(json);
            
                _slackApi.Start();

                _response = _slackGateway.RetrieveDevelopers();
            }
            
            [TearDown]
            public void TearDown()
            {
                _slackApi.Stop();
            }
            
            [Test]
            public void CanSendAGetUsersRequest()
            {
                var receivedRequest = _slackApi.ReceivedRequests.First();
                
                receivedRequest.Url.Should().Be(Address + "api/users.list");
            }
            
            [Test]
            public void CanSendAGetUsersRequestWithAToken()
            {
                var receivedRequest = _slackApi.ReceivedRequests.First();
                
                receivedRequest.Headers["Authorization"].Should().Be("Bearer " + Token);
            }
        
            [Test]
            public void CanGetAListOfSlackDevelopers()
            {
                _response.Should().BeOfType<List<SlackDeveloper>>();
                _response.Should().HaveCount(6);
            }
        
            [Test]
            public void CanGetIdOfSlackDeveloper()
            {
                _response.First().Id.Should().Be("W0123CHAN");
            }
            
            [Test]
            public void CanGetEmailOfSlackDeveloper()
            {
                _response.First().Email.Should().Be("chandler@friends.com");
            }
        }
        
        [TestFixture]
        public class CanExcludeUsers
        {
            private FluentSimulator _slackApi;
            private SlackGateway _slackGateway;
            private IList<SlackDeveloper> _response;

            [SetUp]
            public void SetUp()
            {
                _slackApi = new FluentSimulator(Address);
                _slackGateway = new SlackGateway(Address, Token);
            
                var json = File.ReadAllText(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "../../../Gateway/ApiEndpointResponse/ExcludedSlackUsersResponse.json"
                    )
                );
                
                _slackApi.Get("/api/users.list").Responds(json);
            
                _slackApi.Start();

                _response = _slackGateway.RetrieveDevelopers();
            }
            
            [TearDown]
            public void TearDown()
            {
                _slackApi.Stop();
            }
            
            [Test]
            public void CanSendAGetUsersRequest()
            {
                var receivedRequest = _slackApi.ReceivedRequests.First();
                
                receivedRequest.Url.Should().Be(Address + "api/users.list");
            }
            
            [Test]
            public void CanSendAGetUsersRequestWithAToken()
            {
                var receivedRequest = _slackApi.ReceivedRequests.First();
                
                receivedRequest.Headers["Authorization"].Should().Be("Bearer " + Token);
            }
        
            [Test]
            public void CanGetAListOfSlackDevelopers()
            {
                _response.Should().BeOfType<List<SlackDeveloper>>();
                _response.Should().HaveCount(2);
            }
        
            [Test]
            public void CanGetIdOfSlackDeveloper()
            {
                _response.First().Id.Should().Be("W345JOEY");
            }
            
            [Test]
            public void CanGetEmailOfSlackDeveloper()
            {
                _response.First().Email.Should().Be("joey@friends.com");
            }
        }
    }
}
