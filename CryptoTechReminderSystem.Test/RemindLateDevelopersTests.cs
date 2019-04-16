using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class RemindLateDevelopersTests
    {
        RemindDeveloperSpy _remindDeveloperSpy;
        GetLateDevelopersSpy _getLateDevelopersSpy;
        GetLateDevelopersStub _getLateDevelopersStub;

        [SetUp]
        public void SetUp()
        {
            _remindDeveloperSpy = new RemindDeveloperSpy();
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
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopers,_remindDeveloperSpy, clock);
            
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

            _remindDeveloperSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindAllLateDevelopers()
        {
           GivenSetUpForTenThirty(_getLateDevelopersStub);

            _remindDeveloperSpy.CountCalled.Should().Be(3);
        }
        
        [Test]
        public void CanRemindDevelopersDirectly()
        {
            GivenSetUpForTenThirty(_getLateDevelopersStub);
            
            _remindDeveloperSpy.Channels.Should().BeEquivalentTo(new List<string> {
                "W0123CHAN",
                "W123AMON",
                "W789ROSS" 
            });
        }
        
        [Test]
        public void CanRemindDevelopersDirectlyWithAMessage()
        {
            GivenSetUpForTenThirty(_getLateDevelopersStub);

            _remindDeveloperSpy.Text.Should().Be("TIMESHEETS ARE GOOD YO!");
        }
        
        [Test]
        [TestCase(10,15)]
        [TestCase(14,30)]

        public void CanAvoidRemindingLateDevelopersAtTime(int hour, int minute)
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, hour, minute, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy.CountCalled.Should().Be(0);
        }
        
    }    
}
