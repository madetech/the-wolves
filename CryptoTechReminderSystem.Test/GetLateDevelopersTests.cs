using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class GetLateDevelopersTests
    {
        private class HarvestGatewayStub : ITimesheetAndDeveloperRetriever
        {
            public string FirstName { private get; set; }
            public string LastName { get; set; }
            public int Id { private get; set; }
            public int Hours { get; set; }
            public string Email { get; set; }
            
            public IList<Developer> RetrieveDevelopers()
            {
            
                IList<Developer> result = new List<Developer>
                {
                    new Developer
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Hours = Hours
                    }
                };

                return result;
            }

            public IEnumerable<TimeSheet> RetrieveTimeSheets()
            {
                return null;
            }
        }

        private class HarvestGatewaySpy : ITimesheetAndDeveloperRetriever
        {
            public bool IsCalled;
            public bool TimeSheetHasBeenCalled;
            public IList<Developer> RetrieveDevelopers()
            {
                IsCalled = true;
                IList<Developer> result = new List<Developer>
                {
                    new Developer()
                };

                return result;
            }

            public IEnumerable<TimeSheet> RetrieveTimeSheets()
            {
                TimeSheetHasBeenCalled = true;
                return new List<TimeSheet>();
            }
        }
        
        public class SlackGatewayStub : IMessageSenderAndRetriever
        {
            public string Name { get; set; }
            public int Id { private get; set; }
            public string Email { get; set; }
            public string DisplayName { get; set; }
            public bool isBot { get; set; }
            public IList<Developer> RetrieveDevelopers()
            {
                return new List<Developer>
                {
                    new SlackDeveloper
                    {
                        Name = Name,
                        Id = Id,
                        Email = Email,
                        DisplayName = DisplayName,
                        isBot = isBot
                    }
                };
            }

            public void Send(Message message)
            {
                throw new System.NotImplementedException();
            }
        }

        public class SlackGatewaySpy : IMessageSenderAndRetriever
        {
            public bool DevelopersHasBeenCalled;
            public void Send(Message message)
            {
                throw new System.NotImplementedException();
            }

            public IList<Developer> RetrieveDevelopers()
            {
                DevelopersHasBeenCalled = true;
                return new List<Developer>();
            }
        }
        
        [Test]
        public void CanGetHarvestDevelopers()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var slackGatewaySpy = new SlackGatewaySpy();
            var getDevelopers = new GetLateDevelopers(slackGatewaySpy, harvestGatewaySpy);
            
            getDevelopers.Execute();
            
            harvestGatewaySpy.IsCalled.Should().BeTrue();
        }
        
        
        [Test]
        public void CanGetSlackDevelopers()
        {
            var harvestGatewayStub = new HarvestGatewayStub
            {
                FirstName = "Bob"
            };
            var slackGatewaySpy = new SlackGatewaySpy();
            
            var getDevelopers = new GetLateDevelopers(slackGatewaySpy, harvestGatewayStub);
            
            getDevelopers.Execute();

            slackGatewaySpy.DevelopersHasBeenCalled.Should().BeTrue();
        }
        
        [Test]
        public void CanGetTimesheets()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var slackGatewayStub = new SlackGatewayStub();
            
            var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewaySpy);
            
            getDevelopers.Execute();

            harvestGatewaySpy.TimeSheetHasBeenCalled.Should().BeTrue();
        }
        
        [Test]
        public void CanGetLateDevelopers()
        {
            var harvestGatewayStub = new HarvestGatewayStub();
            var slackGatewayStub = new SlackGatewayStub();
            
            var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewayStub);
            
            var response = getDevelopers.Execute();
            
        }
        
    }

    public class SlackDeveloper : Developer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool isBot { get; set; }
    }
}