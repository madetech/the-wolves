using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
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

            public IList<TimeSheet> RetrieveTimeSheets(DateTimeOffset dateFrom, DateTimeOffset dateTo)
            {
                IList<TimeSheet> result = TimeSheets;
                return result;
            }
        }

        private class HarvestGatewaySpy : IHarvestDeveloperRetriever, ITimeSheetRetriever
        {
            public bool IsRetrieveDevelopersCalled;
            public bool IsRetrieveTimeSheetsCalled;
            public DateTimeOffset[] RetrieveTimeSheetsArguments;
            
            public IList<HarvestDeveloper> RetrieveDevelopers()
            {
                IsRetrieveDevelopersCalled = true;
                IList<HarvestDeveloper> result = new List<HarvestDeveloper>();
              
                return result;
            }

            public IList<TimeSheet> RetrieveTimeSheets(DateTimeOffset dateFrom, DateTimeOffset dateTo)
            {
                IsRetrieveTimeSheetsCalled = true;
            
                RetrieveTimeSheetsArguments = new[]
                {
                    dateFrom,
                    dateTo
                };

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
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            var getDevelopers = new GetLateDevelopers(slackGatewaySpy, harvestGatewaySpy, harvestGatewaySpy, clock);
            
            getDevelopers.Execute();
            
            harvestGatewaySpy.IsRetrieveDevelopersCalled.Should().BeTrue();
        }
        
        
        [Test]
        public void CanGetSlackDevelopers()
        {
            var harvestGatewaySpy = new HarvestGatewaySpy();
            var slackGatewaySpy = new SlackGatewaySpy();
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 10, 30, 0)
                )
            );
            var getDevelopers = new GetLateDevelopers(slackGatewaySpy, harvestGatewaySpy, harvestGatewaySpy, clock);
            
            getDevelopers.Execute();

            slackGatewaySpy.IsRetrieveDevelopersCalled.Should().BeTrue();
        }

        [TestFixture]
        public class CanGetTimesheets
        {
            [Test]
            public void CanRetrieveTimesheets()
            {
                var harvestGatewaySpy = new HarvestGatewaySpy();
                var slackGatewayStub = new SlackGatewayStub();
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );
            
                var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewaySpy, harvestGatewaySpy, clock);
            
                getDevelopers.Execute();

                harvestGatewaySpy.IsRetrieveTimeSheetsCalled.Should().BeTrue();
            }
            
            [Test]
            [TestCase(19)]
            [TestCase(18)]
            [TestCase(17)]
            [TestCase(16)]
            [TestCase(15)]
            public void CanRetrieveTimesheetsWithStartingDate(int day)
            {
                var harvestGatewaySpy = new HarvestGatewaySpy();
                var slackGatewayStub = new SlackGatewayStub();
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 04, day)
                    )
                );
            
                var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewaySpy, harvestGatewaySpy, clock);
            
                getDevelopers.Execute();

                harvestGatewaySpy.RetrieveTimeSheetsArguments[0].Should().Be(
                    new DateTimeOffset(
                        new DateTime(2019, 04, 15)
                    )
                );
            }
            
            [Test]
            [TestCase(08)]
            [TestCase(09)]
            [TestCase(10)]
            [TestCase(11)]
            [TestCase(12)]
            public void CanRetrieveTimesheetsWithEndingDate(int day)
            {
                var harvestGatewaySpy = new HarvestGatewaySpy();
                var slackGatewayStub = new SlackGatewayStub();
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 04, day)
                    )
                );
            
                var getDevelopers = new GetLateDevelopers(slackGatewayStub, harvestGatewaySpy, harvestGatewaySpy, clock);
            
                getDevelopers.Execute();

                harvestGatewaySpy.RetrieveTimeSheetsArguments[1].Should().Be(
                    new DateTimeOffset(
                        new DateTime(2019, 04, 12)
                    )
                );
            }
        }
        
        [TestFixture]
        public class CanGetLateDevelopers
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
            public void CanGetLateDeveloper()
            {
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = 1337 }, 5
                ).ToArray();
                
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );
                
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, clock);
                
                var response = getDevelopers.Execute();
         
                response.Developers.First().Should().Be("U9999");
            }
            
            [Test]
            public void CanGetAnotherLateDeveloper()
            {                
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = 123 }, 5
                ).ToArray();
                
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );

                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, clock);
                
                var response = getDevelopers.Execute();
              
                response.Developers.First().Should().Be("U8723");
            }
        }
    }
}
