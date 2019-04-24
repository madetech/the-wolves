using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class RemindLateDevelopersTests
    {
        private SendReminderSpy _sendReminderSpy;
        private GetLateDevelopersSpy _getLateDevelopersSpy;
        private GetLateDevelopersStub _getLateDevelopersStub;

        [SetUp]
        public void SetUp()
        {
            _sendReminderSpy = new SendReminderSpy();
            _getLateDevelopersSpy = new GetLateDevelopersSpy();
            _getLateDevelopersStub = new GetLateDevelopersStub();  
        }
        
        private void GivenSetUpForTenThirty(IGetLateDevelopers getLateDevelopers)
        {
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopers,_sendReminderSpy, clock);
            
            remindLateDevelopers.Execute(
                new RemindLateDevelopersRequest
                {
                    Message = "TIMESHEETS ARE GOOD YO!"
                }
            );
        }
        
        [Test]
        public void CanGetLateDevelopers()
        {
            GivenSetUpForTenThirty(_getLateDevelopersSpy);
               
            _getLateDevelopersSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindDevelopers()
        {
            GivenSetUpForTenThirty(_getLateDevelopersSpy);

            _sendReminderSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindAllLateDevelopers()
        {
           GivenSetUpForTenThirty(_getLateDevelopersStub);

            _sendReminderSpy.CountCalled.Should().Be(3);
        }
        
        [Test]
        public void CanRemindDevelopersDirectly()
        {
            GivenSetUpForTenThirty(_getLateDevelopersStub);
            
            _sendReminderSpy.Channels.Should().BeEquivalentTo(
                new List<string> {
                    "W0123CHAN",
                    "W123AMON",
                    "W789ROSS"
                }
            );
        }
        
        [Test]
        public void CanRemindDevelopersDirectlyWithAMessage()
        {
            GivenSetUpForTenThirty(_getLateDevelopersStub);

            _sendReminderSpy.Text.Should().Be("TIMESHEETS ARE GOOD YO!");
        }
        
        [Test]
        [TestCase(10,15)]
        [TestCase(14,30)]
        public void CanAvoidRemindingLateDevelopersAtTime(int hour, int minute)
        {
            var remindDeveloperSpy = new SendReminderSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, hour, minute, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy.CountCalled.Should().Be(0);
        }
        
        [Test]
        [TestCase(01, 11, 30, true)]
        [TestCase(02, 11, 30, false)]
        [TestCase(01, 14, 00, false)]
        [TestCase(08, 12, 00, true)]
        public void CanRemindLateDevelopersOnFriday(int day, int hour, int minute, bool expectedOutcome)
        {
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, day, hour, minute, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(_getLateDevelopersStub,_sendReminderSpy, clock);

            remindLateDevelopers.Execute(new RemindLateDevelopersRequest
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            _sendReminderSpy.Called.Should().Be(expectedOutcome);
        }
        
    }    
}
