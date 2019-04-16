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
        private class HarvestGatewayStub : IHarvestDeveloperRetriever, ITimeSheetRetriever
        {
            public HarvestDeveloper[] Developers { get; set; }
            
            public TimeSheet[] TimeSheets { get; set; }
            
            public IList<HarvestDeveloper> RetrieveDevelopers()
            {
                IList<HarvestDeveloper> result = Developers;
                return result;
            }

            public IList<TimeSheet> RetrieveTimeSheets()
            {
                IList<TimeSheet> result = TimeSheets;
                return result;
            }
        }

        private class HarvestGatewaySpy : IHarvestDeveloperRetriever, ITimeSheetRetriever
        {
            public bool IsRetrieveDevelopersCalled;
            public bool IsRetrieveTimeSheetsCalled;
            
            public IList<HarvestDeveloper> RetrieveDevelopers()
            {
                IsRetrieveDevelopersCalled = true;
                IList<HarvestDeveloper> result = new List<HarvestDeveloper>();
              
                return result;
            }

            public IList<TimeSheet> RetrieveTimeSheets()
            {
                IsRetrieveTimeSheetsCalled = true;
                return new List<TimeSheet>();
            }
        }

        private class SlackGatewayStub : ISlackDeveloperRetriever
        {
            public SlackDeveloper[] Developers;

            public IList<SlackDeveloper> RetrieveDevelopers()
            {
                IList<SlackDeveloper> result = Developers;
                return result;
            }
        }

        private class SlackGatewaySpy : ISlackDeveloperRetriever
        {
            public bool IsRetrieveDevelopersCalled;

            public IList<SlackDeveloper> RetrieveDevelopers()
            {
                IsRetrieveDevelopersCalled = true;
                return new List<SlackDeveloper>();
            }
        }
        
        [Test]
        public void CanGetHarvestDevelopers()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var slackGatewaySpy = new SlackGatewaySpy();
            var getDevelopers = new GetLateDevelopers(slackGatewaySpy, harvestGatewaySpy, harvestGatewaySpy);
            
            getDevelopers.Execute();
            
            harvestGatewaySpy.IsRetrieveDevelopersCalled.Should().BeTrue();
        }
        
        
        [Test]
        public void CanGetSlackDevelopers()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var slackGatewaySpy = new SlackGatewaySpy();            
            var getDevelopers = new GetLateDevelopers(slackGatewaySpy, harvestGatewaySpy, harvestGatewaySpy);
            
            getDevelopers.Execute();

            slackGatewaySpy.IsRetrieveDevelopersCalled.Should().BeTrue();
        }
        
        [Test]
        public void CanGetTimesheets()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var slackGatewayStub = new SlackGatewayStub();
            
            var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewaySpy, harvestGatewaySpy);
            
            getDevelopers.Execute();

            harvestGatewaySpy.IsRetrieveTimeSheetsCalled.Should().BeTrue();
        }
        

        [TestFixture()]
        public class TestGetLateUserResponse
        {
            private HarvestGatewayStub _harvestGatewayStub;
            private SlackGatewayStub _slackGatewayStub;
            
            [SetUp]
            public void SetUp()
            {
                _harvestGatewayStub = new HarvestGatewayStub()
                {
                    Developers = new[]
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
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub()
                {
                    Developers = new[]
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
                    }
                };
            }
            
            [Test]
            public void CanGetLateDevelopers()
            {
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = 1337 }, 5
                ).ToArray();
                
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub);
                
                var response = getDevelopers.Execute();
         
                response.Developers.First().Should().Be("U9999");
            }
            
            [Test]
            public void CanGetAnotherLateDeveloper()
            {                
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = 123 }, 5
                ).ToArray();

                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub);
                
                var response = getDevelopers.Execute();
              
                response.Developers.First().Should().Be("U8723");
            }
        }
    }
}