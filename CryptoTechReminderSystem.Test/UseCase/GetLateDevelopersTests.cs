using System;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class GetLateDevelopersTests
    {
        private static HarvestGatewaySpy _harvestGatewaySpy;
        private static SlackGatewaySpy _slackGatewaySpy;

        [SetUp]
        public void SetUp()
        {
            _harvestGatewaySpy = new HarvestGatewaySpy();
            _slackGatewaySpy = new SlackGatewaySpy();
        }
        
        [TestFixture]
        public class CanGetDevelopers
        {
            private ClockStub _clock;
            private GetLateDevelopers _getDevelopers;
            
            [SetUp]
            public void SetUp()
            {
                _harvestGatewaySpy = new HarvestGatewaySpy();
                _slackGatewaySpy = new SlackGatewaySpy();
                _clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );

                _getDevelopers = new GetLateDevelopers(_slackGatewaySpy, _harvestGatewaySpy, _harvestGatewaySpy, _clock);
            }
            
            [Test]
            public void CanGetHarvestDevelopers()
            {
                _getDevelopers.Execute();

                _harvestGatewaySpy.IsRetrieveDevelopersCalled.Should().BeTrue();
            }

            [Test]
            public void CanGetSlackDevelopers()
            {
                _getDevelopers.Execute();

                _slackGatewaySpy.IsRetrieveDevelopersCalled.Should().BeTrue();
            }
        }

        [TestFixture]
        public class CanGetTimeSheets
        {
            private SlackGatewayStub _slackGatewayStub;

            [SetUp]
            public void SetUp()
            {
                _harvestGatewaySpy = new HarvestGatewaySpy();
                _slackGatewayStub = new SlackGatewayStub();
            }
            
            [Test]
            public void CanRetrieveTimeSheets()
            {
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewaySpy, _harvestGatewaySpy, clock);
            
                getDevelopers.Execute();

                _harvestGatewaySpy.IsRetrieveTimeSheetsCalled.Should().BeTrue();
            }
            
            [Test]
            [TestCase(19)]
            [TestCase(18)]
            [TestCase(17)]
            [TestCase(16)]
            [TestCase(15)]
            public void CanRetrieveTimeSheetsWithStartingDate(int day)
            {
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 04, day)
                    )
                );
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewaySpy, _harvestGatewaySpy, clock);
            
                getDevelopers.Execute();

                _harvestGatewaySpy.RetrieveTimeSheetsArguments[0].Should().Be(
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
            public void CanRetrieveTimeSheetsWithEndingDate(int day)
            {
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 04, day)
                    )
                );
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewaySpy, _harvestGatewaySpy, clock);
            
                getDevelopers.Execute();

                _harvestGatewaySpy.RetrieveTimeSheetsArguments[1].Should().Be(
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
                _harvestGatewayStub = new HarvestGatewayStub
                {
                    Developers = new[]
                    {
                        new HarvestDeveloper
                        {   
                            Id = 1337,
                            FirstName = "Fred",
                            LastName = "Flintstone",
                            Email = "fred@fred.com"
                        },
                        new HarvestDeveloper
                        {
                            Id = 123,
                            FirstName = "Joe",
                            LastName = "Bloggs",
                            Email = "Joe@Bloggs.com"
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    Developers = new[]
                    {
                        new SlackDeveloper
                        {
                            Email = "fred@fred.com",
                            Id = "U8723"
                        }, 
                        new SlackDeveloper
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
            }
            
            [Test]
            [TestCase(1337, "U9999")]
            [TestCase(123, "U8723")]
            public void CanGetLateADeveloper(int harvestUserId, string slackUserId)
            {
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = harvestUserId }, 5
                ).ToArray();
                
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, clock);
                var response = getDevelopers.Execute();
         
                response.Developers.First().Should().Be(slackUserId);
            }
        } 

        [TestFixture]
        public class CanHandleNoMatches
        {
            private HarvestGatewayStub _harvestGatewayStub;
            private SlackGatewayStub _slackGatewayStub;
            private ClockStub _clock;

            [SetUp]
            public void Setup()
            {
                _clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );
                
                _harvestGatewayStub = new HarvestGatewayStub
                {
                    Developers = new[]
                    {
                        new HarvestDeveloper
                        {   
                            Id = 1337,
                            FirstName = "Fred",
                            LastName = "Flintstone",
                            Email = "fred@fred.com"
                        },
                        new HarvestDeveloper
                        {
                            Id = 123,
                            FirstName = "Joe",
                            LastName = "Bloggs",
                            Email = "Joe@Bloggs.com"
                        },
                        new HarvestDeveloper
                        {
                            Id = 101,
                            FirstName = "Jimbob",
                            LastName = "BaconBath",
                            Email = "JBB@aol.com"
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    Developers = new[]
                    {
                        new SlackDeveloper
                        {
                            Email = "fred@fred.com",
                            Id = "U8723"
                        }, 
                        new SlackDeveloper
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
            }
            
            [Test]
            public void CanHandleWhenCannotFindMatchingSlackDeveloper()
            {
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = 123 }, 5
                ).ToArray();
                
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, _clock);
                var response = getDevelopers.Execute();

                response.Developers.First().Should().Be("U8723");
            }
            
            [Test]
            public void CanHandleWhenNoMatchesAreFound()
            {
                _harvestGatewayStub = new HarvestGatewayStub
                {
                    Developers = new[]
                    {
                        new HarvestDeveloper
                        {
                            Id = 101,
                            FirstName = "Jimbob",
                            LastName = "BaconBath",
                            Email = "JBB@aol.com"
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    Developers = new[]
                    {
                        new SlackDeveloper
                        {
                            Email = "fred@fred.com",
                            Id = "U8723"
                        }, 
                        new SlackDeveloper
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
                
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 0, UserId = 444 }, 5
                ).ToArray();
                
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, _clock);
                var response = getDevelopers.Execute();
                
                response.Developers.Count.Should().Be(0);
            }
            
            [Test]
            public void CanHandleWhenEmailsAreNotExact()
            {
                _harvestGatewayStub = new HarvestGatewayStub
                {
                    Developers = new[]
                    {
                        new HarvestDeveloper
                        {
                            Id = 101,
                            FirstName = "Fred",
                            LastName = "Flintstone",
                            Email = "freddy@aol.com"
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    Developers = new[]
                    {
                        new SlackDeveloper
                        {
                            Email = "freddy@aol.co.uk",
                            Id = "U8723"
                        }, 
                        new SlackDeveloper
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
                
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 0, UserId = 444 }, 5
                ).ToArray();
                
                var getDevelopers = new GetLateDevelopers(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, _clock);
                var response = getDevelopers.Execute();
                
                response.Developers.Count.Should().Be(1);
                response.Developers.First().Should().Be("U8723");
            }
        }
    }
}
