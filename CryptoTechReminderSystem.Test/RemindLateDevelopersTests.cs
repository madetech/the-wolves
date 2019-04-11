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
            public bool _called { private set; get; }
            public int _countCalled{ private set; get; }
            

            public void Execute(RemindDeveloperRequest remindDeveloperRequest)
            {
                _called = true;
                _countCalled++;
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

            remindDeveloperSpy._called.Should().BeTrue(); 
            
        }
        
        [Test]
        public void CanDoThing()
        {
            var remindDeveloperSpy = new RemindDeveloperSpy();
            var getLateDevelopersStub = new GetLateDevelopersStub();
            var clock = new Clock();
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopersStub,remindDeveloperSpy, clock);
            
            remindLateDevelopers.Execute(new RemindLateDevelopersRequest()
            {
                Message = "TIMESHEETS ARE GOOD YO!"
            });

            remindDeveloperSpy._countCalled.Should().Be(3);

        }
        

    }

    public class GetLateDevelopersStub : IGetLateDevelopers
    {
        public GetLateDevelopersResponse Execute()
        {
            return new GetLateDevelopersResponse()
            {
                Developers = new List<string>()
                {
                    "W0123CHAN",
                    "W123AMON",
                    "W789ROSS"
                }
            };
        }
    }

    
}