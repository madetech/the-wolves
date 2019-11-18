using System;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class ListLateDevelopersTests
    {
        private SendReminderSpy _sendReminderSpy;
        private GetLateDevelopersSpy _getLateDevelopersSpy;
        private ClockStub _clock;

        [SetUp]
        public void SetUp()
        {
            _sendReminderSpy = new SendReminderSpy();
            _getLateDevelopersSpy = new GetLateDevelopersSpy();
            _clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 13, 30, 0)
                )
            );
        }
        
        [Test]
        public void CanGetLateDevelopers()
        {
            var listLateDevelopers = new ListLateDevelopers(_getLateDevelopersSpy, _sendReminderSpy);

            listLateDevelopers.Execute(new ListLateDevelopersRequest());

            _getLateDevelopersSpy.Called.Should().BeTrue();
        }

        [Test]
        public void CanRemindDevelopers()
        {
            var listLateDevelopers = new ListLateDevelopers(_getLateDevelopersSpy, _sendReminderSpy);

            listLateDevelopers.Execute(
                new ListLateDevelopersRequest
                {
                    LateDevelopersMessage = "TIMESHEETS ARE GOOD YO!"
                }
            );

            _sendReminderSpy.Called.Should().BeTrue();
        }

        [Test]
        [TestCase( "W0123CHAN", "W123AMON", "W789ROSS")]
        [TestCase( "W0123CHAN", "W123AMON")]
        public void CanCheckLateDevelopersMessageHasAllUsers(params string[] userId)
        {
            var getLateDevelopersStub = new GetLateDevelopersStub(userId.ToList());
            var listLateDevelopers = new ListLateDevelopers(getLateDevelopersStub, _sendReminderSpy);
            var expectedLateDevelopersMessage = "TIMESHEETS ARE GOOD YO!";

            listLateDevelopers.Execute(
                new ListLateDevelopersRequest
                {
                    LateDevelopersMessage = expectedLateDevelopersMessage
                }
            );

            expectedLateDevelopersMessage = userId.Aggregate(expectedLateDevelopersMessage, (current, user) => current + $"\n• <@{user}>");

            _sendReminderSpy.Text.Should().Be(expectedLateDevelopersMessage);
        }

        [Test]
        public void CanCheckMessageHadChannel()
        {
            var listLateDevelopers = new ListLateDevelopers(_getLateDevelopersSpy, _sendReminderSpy);

            const string expectedChannel = "CH123456";
            
            listLateDevelopers.Execute(
                new ListLateDevelopersRequest
                {
                    Channel = expectedChannel
                }
            );
            
            _sendReminderSpy.Channels.First().Should().Be(expectedChannel);
        }

        [Test]
        public void CanSendNoLateDevelopersMessageWhenNoLateDevelopers()
        {
            var getLateDevelopersEmptyStub = new GetLateDevelopersEmptyStub();
            var listLateDevelopers = new ListLateDevelopers(getLateDevelopersEmptyStub, _sendReminderSpy);
            const string expectedNoLateDevelopersMessage = "Everyone has submitted their timesheets!";

            listLateDevelopers.Execute(
                new ListLateDevelopersRequest
                {
                    NoLateDevelopersMessage = expectedNoLateDevelopersMessage
                }
            );

            _sendReminderSpy.Text.Should().Be(expectedNoLateDevelopersMessage);
        }
    }
}
