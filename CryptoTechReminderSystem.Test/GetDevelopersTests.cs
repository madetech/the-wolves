using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class GetDevelopersTests
    {
        private class HarvestGatewayStub : ITimesheetAndDeveloperRetriever
        {
            public string FirstName { private get; set; }
            
            public IList<Developer> RetrieveDevelopers()
            {
            
                IList<Developer> result = new List<Developer>()
                {
                    new Developer()
                    {
                        FirstName = FirstName
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
                IList<Developer> result = new List<Developer>()
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
            public string FirstName { get; set; }
            public IList<Developer> RetrieveDevelopers()
            {
                return new List<Developer>();

            }

            public void Send(Message message)
            {
                throw new System.NotImplementedException();
            }
        }

        public class SlackGatewaySpy : IMessageSenderAndRetriever
        {
            public bool hasBeenCalled;
            public void Send(Message message)
            {
                throw new System.NotImplementedException();
            }

            public IList<Developer> RetrieveDevelopers()
            {
                hasBeenCalled = true;
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
            
            var response = getDevelopers.Execute();

            slackGatewaySpy.hasBeenCalled.Should().BeTrue();
        }
        
        [Test]
        public void CanGetTimesheets()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var slackGatewayStub = new SlackGatewayStub();
            
            var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewaySpy);
            
            var response = getDevelopers.Execute();

            harvestGatewaySpy.TimeSheetHasBeenCalled.Should().BeTrue();
        }
        
    }

}