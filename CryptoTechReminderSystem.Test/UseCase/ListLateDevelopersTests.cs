using System;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class ListLateBillablePeopleTests
    {
        private SendReminderSpy _sendReminderSpy;
        private GetLateBillablePeopleSpy _getLateBillablePeopleSpy;
        private ClockStub _clock;

        [SetUp]
        public void SetUp()
        {
            _sendReminderSpy = new SendReminderSpy();
            _getLateBillablePeopleSpy = new GetLateBillablePeopleSpy();
            _clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 13, 30, 0)
                )
            );
        }
        
        [Test]
        public void CanGetLateBillablePeople()
        {
            var listLateBillablePeople = new ListLateBillablePeople(_getLateBillablePeopleSpy, _sendReminderSpy);

            listLateBillablePeople.Execute(new ListLateBillablePeopleRequest());

            _getLateBillablePeopleSpy.Called.Should().BeTrue();
        }

        [Test]
        public void CanRemindDevelopers()
        {
            var listLateBillablePeople = new ListLateBillablePeople(_getLateBillablePeopleSpy, _sendReminderSpy);

            listLateBillablePeople.Execute(
                new ListLateBillablePeopleRequest
                {
                    LateBillablePeopleMessage = "TIMESHEETS ARE GOOD YO!"
                }
            );

            _sendReminderSpy.Called.Should().BeTrue();
        }

        [Test]
        [TestCase( "W0123CHAN", "W123AMON", "W789ROSS")]
        [TestCase( "W0123CHAN", "W123AMON")]
        public void CanCheckLateBillablePeopleMessageHasAllUsers(params string[] userId)
        {
            var GetLateBillablePeopleStub = new GetLateBillablePeopleStub(userId.ToList());
            var listLateBillablePeople = new ListLateBillablePeople(GetLateBillablePeopleStub, _sendReminderSpy);
            var expectedLateBillablePeopleMessage = "TIMESHEETS ARE GOOD YO!";

            listLateBillablePeople.Execute(
                new ListLateBillablePeopleRequest
                {
                    LateBillablePeopleMessage = expectedLateBillablePeopleMessage
                }
            );

            expectedLateBillablePeopleMessage = userId.Aggregate(expectedLateBillablePeopleMessage, (current, user) => current + $"\n• <@{user}>");

            _sendReminderSpy.Text.Should().Be(expectedLateBillablePeopleMessage);
        }

        [Test]
        public void CanCheckMessageHadChannel()
        {
            var listLateBillablePeople = new ListLateBillablePeople(_getLateBillablePeopleSpy, _sendReminderSpy);

            const string expectedChannel = "CH123456";
            
            listLateBillablePeople.Execute(
                new ListLateBillablePeopleRequest
                {
                    Channel = expectedChannel
                }
            );
            
            _sendReminderSpy.Channels.First().Should().Be(expectedChannel);
        }

        [Test]
        public void CanSendNoLateBillablePeopleMessageWhenNoLateBillablePeople()
        {
            var getLateBillablePeopleEmptyStub = new GetLateBillablePeopleEmptyStub();
            var listLateBillablePeople = new ListLateBillablePeople(getLateBillablePeopleEmptyStub, _sendReminderSpy);
            const string expectedNoLateBillablePeopleMessage = "Everyone has submitted their timesheets!";

            listLateBillablePeople.Execute(
                new ListLateBillablePeopleRequest
                {
                    NoLateBillablePeopleMessage = expectedNoLateBillablePeopleMessage
                }
            );

            _sendReminderSpy.Text.Should().Be(expectedNoLateBillablePeopleMessage);
        }
    }
}
