using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class RemindLateBillablePeopleTests
    {
        private SendReminderSpy _sendReminderSpy;
        private GetLateBillablePeopleSpy _getLateBillablePeopleSpy;
        private GetLateBillablePeopleStub _getLateBillablePeopleStub;

        [SetUp]
        public void SetUp()
        {
            _sendReminderSpy = new SendReminderSpy();
            _getLateBillablePeopleSpy = new GetLateBillablePeopleSpy();
            _getLateBillablePeopleStub = new GetLateBillablePeopleStub();  
        }
        
        private void HandleSetUp(IGetLateBillablePeople GetLateBillablePeople)
        {
            var remindLateBillablePeople = new RemindLateBillablePeople(GetLateBillablePeople, _sendReminderSpy);
            
            remindLateBillablePeople.Execute(
                new RemindLateBillablePeopleRequest
                {
                    Message = "TIMESHEETS ARE GOOD YO!"
                }
            );
        }
        
        [Test]
        public void CanGetLateBillablePeople()
        {
            HandleSetUp(_getLateBillablePeopleSpy);
               
            _getLateBillablePeopleSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindDevelopers()
        {
            HandleSetUp(_getLateBillablePeopleStub);

            _sendReminderSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindAllLateBillablePeople()
        {
           HandleSetUp(_getLateBillablePeopleStub);

            _sendReminderSpy.CountCalled.Should().Be(3);
        }
        
        [Test]
        public void CanRemindDevelopersDirectly()
        {
            HandleSetUp(_getLateBillablePeopleStub);
            
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
            HandleSetUp(_getLateBillablePeopleStub);

            _sendReminderSpy.Text.Should().Be("TIMESHEETS ARE GOOD YO!");
        }   
    }    
}
