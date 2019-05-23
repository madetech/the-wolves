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
        
        private void HandleSetUp(IGetLateDevelopers getLateDevelopers)
        {
            var remindLateDevelopers = new RemindLateDevelopers(getLateDevelopers, _sendReminderSpy);
            
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
            HandleSetUp(_getLateDevelopersSpy);
               
            _getLateDevelopersSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindDevelopers()
        {
            HandleSetUp(_getLateDevelopersStub);

            _sendReminderSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindAllLateDevelopers()
        {
           HandleSetUp(_getLateDevelopersStub);

            _sendReminderSpy.CountCalled.Should().Be(3);
        }
        
        [Test]
        public void CanRemindDevelopersDirectly()
        {
            HandleSetUp(_getLateDevelopersStub);
            
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
            HandleSetUp(_getLateDevelopersStub);

            _sendReminderSpy.Text.Should().Be("TIMESHEETS ARE GOOD YO!");
        }   
    }    
}
