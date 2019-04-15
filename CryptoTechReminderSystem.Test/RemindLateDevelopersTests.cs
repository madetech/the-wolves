using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public partial class RemindLateDevelopersTests
    {
        [Test]
        public void CanGetLateDevelopers()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersSpy = new GetLateDevelopersSpy();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersSpy,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(
                new RemindLateDevelopersRequest()
                {
                    Message = "TIMESHEETS ARE GOOD YO!"
                }
            );

            getLateDevelopersSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindDevelopers()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersSpy = new GetLateDevelopersSpy();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersSpy,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(
                new RemindLateDevelopersRequest
                {
                    Message = "TIMESHEETS ARE GOOD YO!"
                }
            );

            remindDeveloperSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindAllLateDevelopers()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(
                new RemindLateDevelopersRequest
                {
                    Message = "TIMESHEETS ARE GOOD YO!"
                }
            );

            remindDeveloperSpy.CountCalled.Should().Be(3);
        }
        
        [Test]
        public void CanRemindDevelopersDirectly()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy.Channels.Should().BeEquivalentTo(new List<string> {
                "W0123CHAN",
                "W123AMON",
                "W789ROSS" 
            });
        }
        
        [Test]
        public void CanRemindDevelopersDirectlyWithAMessage()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy.Text.Should().Be("TIMESHEETS ARE GOOD YO!");
        }
        
        [Test]
        public void CanAvoidRemindingLateDevelopersAtTenFifteen()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 15, 0)
                )
            );
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy.CountCalled.Should().Be(0);
        }
        
        [Test]
        public void CanAvoidRemindingLateDevelopersAfterOneThirty()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 14, 30, 0)
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
