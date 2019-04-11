using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class RemindLateDevelopersTests
    {
        
        public class Clock : IClock
        {
            public DateTimeOffset Now()
            {
                throw new NotImplementedException();
            }
        }

        public class GetLateDevelopersSpy : IGetLateDevelopers
        {
            public bool _called { private set; get; }

            public GetLateDevelopersResponse Execute()
            {
                _called = true;
                return new GetLateDevelopersResponse()
                {
                    Developers = new List<string>()
                    {
                        "Uncle Craig"
                    }
                };
            }
        }

        public class RemindDeveloperSpy : IRemindDeveloper
        {
            public bool Called { private set; get; }
            public int CountCalled{ private set; get; }

            public List<string> Channels{ private set; get; }
            public string Text{ private set; get; }

            public RemindDeveloperSpy()
            {
                Channels = new List<string>();
            }
            public void Execute(RemindDeveloperRequest remindDeveloperRequest)
            {
                Called = true;
                CountCalled++;
                Channels.Add(remindDeveloperRequest.Channel);
                Text = remindDeveloperRequest.Text;
            }
        }
        
        
        [Test]
        public void CanGetLateDevelopers()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersSpy = new GetLateDevelopersSpy();
            var clock = new Clock();
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersSpy,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            getLateDevelopersSpy._called.Should().BeTrue(); 
            
        }
        
        [Test]
        public void CanRemindDevelopers()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersSpy = new GetLateDevelopersSpy();
            var clock = new Clock();
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersSpy,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy.Called.Should().BeTrue(); 
            
        }
        
        [Test]
        public void CanRemindAllLateDevelopers()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new Clock();
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy.CountCalled.Should().Be(3);
        }
        
        [Test]
        public void CanRemindDevelopersDirectly()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new Clock();
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
            var clock = new Clock();
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy.Text.Should().Be("TIMESHEETS ARE GOOD YO!");
        }
    }

    public class GetLateDevelopersStub : IGetLateDevelopers
    {
        public GetLateDevelopersResponse Execute()
        {
            return new GetLateDevelopersResponse
            {
                Developers = new List<string>
                {
                    "W0123CHAN",
                    "W123AMON",
                    "W789ROSS"
                }
            };
        }
    }

    
}