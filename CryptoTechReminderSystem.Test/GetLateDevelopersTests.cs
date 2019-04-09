using System;
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
            public HarvestDeveloper[] Developers { get; set; }
            
            public TimeSheet[] TimeSheets { get; set; }
            
            public IList<HarvestDeveloper> RetrieveDevelopers()
            {

                IList<HarvestDeveloper> result = Developers;
                return result;
            }

            public IEnumerable<TimeSheet> RetrieveTimeSheets()
            {
                IList<TimeSheet> result = TimeSheets;
                return result;
            }
        }

        private class HarvestGatewaySpy : ITimesheetAndDeveloperRetriever
        {
            public bool IsCalled;
            public bool TimeSheetHasBeenCalled;
            public IList<HarvestDeveloper> RetrieveDevelopers()
            {
                IsCalled = true;
                IList<HarvestDeveloper> result = new List<HarvestDeveloper>();
              
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
            public SlackDeveloper[] Developers;
            public string Name { get; set; }
            public int Id { private get; set; }
            public string Email { get; set; }
            public string DisplayName { get; set; }
            public bool isBot { get; set; }
            public IList<SlackDeveloper> RetrieveDevelopers()
            {
                IList<SlackDeveloper> result = Developers;
                return result;
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

            public IList<SlackDeveloper> RetrieveDevelopers()
            {
                DevelopersHasBeenCalled = true;
                return new List<SlackDeveloper>();
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
            var harvestGatewaySpy = new HarvestGatewaySpy();
        
            var slackGatewaySpy = new SlackGatewaySpy();
            
            var getDevelopers = new GetLateDevelopers(slackGatewaySpy, harvestGatewaySpy);
            
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
            
            harvestGatewayStub.Developers = new HarvestDeveloper[]
            {
                new HarvestDeveloper()
                {   
                    Id = 1337,
                    FirstName = "Fred",
                    LastName = "Flintstone",
                    Email = "fred@fred.com",
                },
                new HarvestDeveloper()
                {
                    Id = 123,
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = "Joe@Bloggs.com"
                },
            };
            
            harvestGatewayStub.TimeSheets = new TimeSheet[]
            {
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 1337
                }, 
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 1337
                }, 
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 1337
                }, 
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 1337
                }, 
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 1337
                }, 
            };
            
            slackGatewayStub.Developers = new SlackDeveloper[]
            {
                new SlackDeveloper
                {
                    Email = "fred@fred.com",
                    Id = "U8723",
                }, 
                new SlackDeveloper
                {
                    Email = "Joe@Bloggs.com",
                    Id = "U9999",
                }
            };
            
            
            
            var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewayStub);
            
            var response = getDevelopers.Execute();
     
          
            response.Developers.First().Should().Be("U9999");

        }
        
        [Test]
        public void CanGetAnotherLateDeveloper()
        {
            var harvestGatewayStub = new HarvestGatewayStub();
            var slackGatewayStub = new SlackGatewayStub();
            
            harvestGatewayStub.Developers = new HarvestDeveloper[]
            {
                new HarvestDeveloper()
                {   
                    Id = 1337,
                    FirstName = "Fred",
                    LastName = "Flintstone",
                    Email = "fred@fred.com",
                },
                new HarvestDeveloper()
                {
                    Id = 123,
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = "Joe@Bloggs.com"
                },
            };
            
            harvestGatewayStub.TimeSheets = new TimeSheet[]
            {
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 123
                }, 
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 123
                }, 
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 123
                }, 
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 123
                }, 
                new TimeSheet()
                {
                    Hours = 7,
                    UserId = 123
                }, 
            };
            
            slackGatewayStub.Developers = new SlackDeveloper[]
            {
                new SlackDeveloper
                {
                    Email = "fred@fred.com",
                    Id = "U8723",
                }, 
                new SlackDeveloper
                {
                    Email = "Joe@Bloggs.com",
                    Id = "U9999",
                }
            };
            
            
            
            var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewayStub);
            
            var response = getDevelopers.Execute();
          
            response.Developers.First().Should().Be("U8723");

        }
        
    }
}