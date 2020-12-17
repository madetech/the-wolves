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
        private const string HarvestApiAddress = "http://localhost:8010/";
        private const string SlackApiUsersPath = "api/users.list";
        private const string SlackApiPostMessagePath = "api/chat.postMessage";
        private const string HarvestApiUsersPath = "/api/v2/users";
        private static FluentSimulator _slackApi;
        private static FluentSimulator _harvestApi;
        private static HarvestGateway _harvestGateway;
        private static SlackGateway _slackGateway;
        private static SendReminder _sendReminder;
        private const string BillablePersonRoles = 
            "Software Engineer, Senior Software Engineer, Senior Engineer, Lead Engineer, " +
            "Delivery Manager, SRE, Consultant, Delivery Principal";

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
            _harvestApi = new FluentSimulator(HarvestApiAddress);
            _harvestGateway = new HarvestGateway(
                HarvestApiAddress, 
                "xxxx-xxxxxxxxx-xxxx",
                "234567",
                "The Wolves",
                BillablePersonRoles
            );
            _sendReminder = new SendReminder(_slackGateway);
            
            var slackGetUsersResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../ApiEndpointResponse/SlackUsersResponse.json"
                )
            );

            _slackApi.Get("/" + SlackApiUsersPath).Responds(slackGetUsersResponse);

            _slackApi.Post("/" + SlackApiPostMessagePath).Responds(new { ok = true });
            
            var harvestGetUsersResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../ApiEndpointResponse/HarvestUsersResponse.json"
                )
            );
            
            _harvestApi.Get(HarvestApiUsersPath).Responds(harvestGetUsersResponse);
            
            var harvestGetTimeEntriesResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../ApiEndpointResponse/HarvestTimeEntriesResponse.json"
                )
            );
            
            var harvestGetTimeEntriesResponseEndOfTheMonth = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../ApiEndpointResponse/HarvestTimeEntriesEndOfTheMonthResponse.json"
                )
            );

            var harvestGetProject1UserAssignmentsResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../ApiEndpointResponse/HarvestProject1UserAssignmentsResponse.json"
                )
            );
            
            var harvestGetProject2UserAssignmentsResponse = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../../../ApiEndpointResponse/HarvestProject2UserAssignmentsResponse.json"
                )
            );
            
            _harvestApi.Get("/api/v2/time_entries")
                .WithParameter("from", "2019-02-25")
                .WithParameter("to", "2019-03-01")
                .WithParameter("page", "1")
                .Responds(harvestGetTimeEntriesResponse);
            
            _harvestApi.Get("/api/v2/time_entries")
                .WithParameter("from", "2019-07-29")
                .WithParameter("to", "2019-07-31")
                .WithParameter("page", "1")
                .Responds(harvestGetTimeEntriesResponseEndOfTheMonth);

            _harvestApi.Get("/api/v2/projects/26670539/user_assignments")
                .WithParameter("page", "1")
                .Responds(harvestGetProject1UserAssignmentsResponse);
            
            _harvestApi.Get("/api/v2/projects/26670540/user_assignments")
                .WithParameter("page", "1")
                .Responds(harvestGetProject2UserAssignmentsResponse);

            _slackApi.Start();
            _harvestApi.Start();
        }

        [SetUp]
        public void Setup()
        {
            HandleSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            _harvestApi.Stop();
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
            
            var getLateBillablePeople = new GetLateBillablePeople(_slackGateway, _harvestGateway, _harvestGateway, clock);

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
            
            var getLateBillablePeople = new GetLateBillablePeople(_slackGateway, _harvestGateway, _harvestGateway, clock);
            
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
        
        [Test]
        public void CanListLateBillablePeople()
        {
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 13, 30, 0)
                )
            );
            
            var getLateBillablePeople = new GetLateBillablePeople(_slackGateway, _harvestGateway, _harvestGateway, clock);

            var listLateBillablePeople = new ListLateBillablePeople(getLateBillablePeople, _sendReminder);

            const string lateBillablePeopleMessage = "These are the people yet to submit time sheets:";
            const string channel = "CHBUZLJT1";
            
            listLateBillablePeople.Execute(
                new ListLateBillablePeopleRequest
                {
                    LateBillablePeopleMessage = lateBillablePeopleMessage,
                    Channel = channel
                }
            );
            
            var lastSlackApiRequest = JObject.Parse(_slackApi.ReceivedRequests.Last().RequestBody);

            lastSlackApiRequest["channel"].ToString().Should().Be(channel);

            var expectedMessage = $"{lateBillablePeopleMessage}\n• <@W123AROB>\n• <@W345ABAT>\n• <@W345ALFR>";
            lastSlackApiRequest["text"].ToString().Should().Be(expectedMessage); 
        }
        
        public void CanRemindProjectManagers()
        {
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 13, 00, 0)
                    )
            );

            var getProjectManagersWithOpenTimeEntries =
                new GetProjectManagersWithOpenTimeEntries(_slackGateway, _harvestGateway, _harvestGateway, clock);

            var remindProjectManagers = new RemindProjectManagers(getProjectManagersWithOpenTimeEntries, _sendReminder);

            remindProjectManagers.Execute(
                new RemindLateBillablePeopleRequest
                {
                    Message = "You have some approving to do on Harvest."
                }
            );
            
            _slackApi.ReceivedRequests.Should()
                .Contain(request => request.RawUrl.ToString() == "/" + SlackApiUsersPath);
            /*
             Bruce Wayne has two time entries where `is_closed` == false across two projects.
             The Wolves should therefore send a reminder to the project manager of each project.
             2 x reminders in total. 
             */
            _slackApi.ReceivedRequests.Count(request => request.RawUrl.ToString() == "/" + SlackApiPostMessagePath)
                .Should().Be(2);
        }
    }
}
