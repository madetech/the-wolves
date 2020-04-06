using System;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class GetLateBillablePeopleTests
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
        public class CanGetBillablePeople
        {
            private ClockStub _clock;
            private GetLateBillablePeople _getBillablePeople;
            
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

                _getBillablePeople = new GetLateBillablePeople(_slackGatewaySpy, _harvestGatewaySpy, _harvestGatewaySpy, _clock);
            }
            
            [Test]
            public void CanGetHarvestBillablePeople()
            {
                _getBillablePeople.Execute();

                _harvestGatewaySpy.IsRetrieveBillablePeopleCalled.Should().BeTrue();
            }

            [Test]
            public void CanGetSlackBillablePeople()
            {
                _getBillablePeople.Execute();

                _slackGatewaySpy.IsRetrieveBillablePeopleCalled.Should().BeTrue();
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
                var getBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewaySpy, _harvestGatewaySpy, clock);
            
                getBillablePeople.Execute();

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
                var getBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewaySpy, _harvestGatewaySpy, clock);
            
                getBillablePeople.Execute();

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
                var getBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewaySpy, _harvestGatewaySpy, clock);
            
                getBillablePeople.Execute();

                _harvestGatewaySpy.RetrieveTimeSheetsArguments[1].Should().Be(
                    new DateTimeOffset(
                        new DateTime(2019, 04, 12)
                    )
                );
            }
        }
        
        [TestFixture]
        public class CanGetLateBillablePeople
        {
            private HarvestGatewayStub _harvestGatewayStub;
            private SlackGatewayStub _slackGatewayStub;
            
            [SetUp]
            public void SetUp()
            {
                _harvestGatewayStub = new HarvestGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new HarvestBillablePerson
                        {   
                            Id = 1337,
                            FirstName = "Fred",
                            LastName = "Flintstone",
                            Email = "fred@fred.com",
                            WeeklyHours = 35
                        },
                        new HarvestBillablePerson
                        {
                            Id = 123,
                            FirstName = "Joe",
                            LastName = "Bloggs",
                            Email = "Joe@Bloggs.com",
                            WeeklyHours = 28
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new SlackBillablePerson
                        {
                            Email = "fred@fred.com",
                            Id = "U8723"
                        }, 
                        new SlackBillablePerson
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
            }
            
            [Test]
            [TestCase(1337, 5, "U8723", "U9999")]
            [TestCase(123, 4,"U9999", "U8723")]
            public void CanGetLateABillablePerson(int harvestUserId, int capacityInDays,string submittingUserSlackUserId, string lateUsersSlackUserId)
            {
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = harvestUserId }, capacityInDays
                ).ToArray();
                
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );
                var getLateBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, clock);
                var response = getLateBillablePeople.Execute();
                
                response.BillablePeople.Any(billablePerson => billablePerson.Id == lateUsersSlackUserId).Should().BeTrue();
                response.BillablePeople.Any(billablePerson => billablePerson.Id == submittingUserSlackUserId).Should().BeFalse();
            }
            
            [Test]
            [TestCase(1337, 0, "U8723", "U9999")]
            [TestCase(123, 1, "U9999", "U8723")]
            public void CanGetLateABillablePersonMidweek(int harvestUserId, int nonWorkedWeekDayForPartTime, string submittingUserSlackUserId, string lateUsersSlackUserId)
            {
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 04, 30, 10, 30, 0)
                    )
                );
                
                var workingDaysOnTheWeekByEndOfTheMonth = clock.Now().DayOfWeek - DayOfWeek.Monday + 1;
                var expectedDays = workingDaysOnTheWeekByEndOfTheMonth - nonWorkedWeekDayForPartTime;
                
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = harvestUserId }, expectedDays
                ).ToArray();
                
                var getLateBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, clock);
                var response = getLateBillablePeople.Execute();
                
                response.BillablePeople.Should().Contain(billablePerson => billablePerson.Id == lateUsersSlackUserId);
                response.BillablePeople.Should().NotContain(billablePerson => billablePerson.Id == submittingUserSlackUserId);
            }
            
            [Test]
            public void CanNotGetLateBillablePeopleOnWeekend()
            {
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = 000 }, 2
                ).ToArray();
                
                var clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 08, 31, 10, 30, 0)
                    )
                );
                
                var getLateBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, clock);
                var response = getLateBillablePeople.Execute();

                response.BillablePeople.Should().HaveCount(0);
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
                    BillablePeople = new[]
                    {
                        new HarvestBillablePerson
                        {   
                            Id = 1337,
                            FirstName = "Fred",
                            LastName = "Flintstone",
                            Email = "fred@fred.com",
                            WeeklyHours = 35
                        },
                        new HarvestBillablePerson
                        {
                            Id = 123,
                            FirstName = "Joe",
                            LastName = "Bloggs",
                            Email = "Joe@Bloggs.com",
                            WeeklyHours = 35
                        },
                        new HarvestBillablePerson
                        {
                            Id = 101,
                            FirstName = "Jimbob",
                            LastName = "BaconBath",
                            Email = "JBB@aol.com",
                            WeeklyHours = 35
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new SlackBillablePerson
                        {
                            Email = "fred@fred.com",
                            Id = "U8723"
                        }, 
                        new SlackBillablePerson
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
            }
            
            [Test]
            public void CanHandleWhenCannotFindMatchingSlackBillablePerson()
            {
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 7, UserId = 123 }, 5
                ).ToArray();
                
                var getBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, _clock);
                var response = getBillablePeople.Execute();

                response.BillablePeople.First().Id.Should().Be("U8723");
            }
            
            [Test]
            public void CanHandleWhenNoMatchesAreFound()
            {
                _harvestGatewayStub = new HarvestGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new HarvestBillablePerson
                        {
                            Id = 101,
                            FirstName = "Jimbob",
                            LastName = "BaconBath",
                            Email = "JBB@aol.com",
                            WeeklyHours = 35
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new SlackBillablePerson
                        {
                            Email = "fred@fred.com",
                            Id = "U8723"
                        }, 
                        new SlackBillablePerson
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
                
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 0, UserId = 444 }, 5
                ).ToArray();
                
                var getBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, _clock);
                var response = getBillablePeople.Execute();
                
                response.BillablePeople.Count.Should().Be(0);
            }
            
            [Test]
            public void CanHandleWhenEmailsAreNotExact()
            {
                _harvestGatewayStub = new HarvestGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new HarvestBillablePerson
                        {
                            Id = 101,
                            FirstName = "Fred",
                            LastName = "Flintstone",
                            Email = "freddy@aol.com",
                            WeeklyHours = 35
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new SlackBillablePerson
                        {
                            Email = "freddy@aol.co.uk",
                            Id = "U8723"
                        }, 
                        new SlackBillablePerson
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
                
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 0, UserId = 444 }, 5
                ).ToArray();
                
                var getBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, _clock);
                var response = getBillablePeople.Execute();
                
                response.BillablePeople.Count.Should().Be(1);
                response.BillablePeople.First().Id.Should().Be("U8723");
            }
            
            
            [Test]
            public void CanHandleWhenEmailsAreNotInTheSameCase()
            {
                _harvestGatewayStub = new HarvestGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new HarvestBillablePerson
                        {
                            Id = 101,
                            FirstName = "Fred",
                            LastName = "Flintstone",
                            Email = "freddy@aol.com",
                            WeeklyHours = 35
                        }
                    }
                };
                
                _slackGatewayStub = new SlackGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new SlackBillablePerson
                        {
                            Email = "Freddy@Aol.com",
                            Id = "U8723"
                        }, 
                        new SlackBillablePerson
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        }
                    }
                };
                
                _harvestGatewayStub.TimeSheets = Enumerable.Repeat(
                    new TimeSheet { Hours = 0, UserId = 444 }, 5
                ).ToArray();
                
                var getBillablePeople = new GetLateBillablePeople(_slackGatewayStub, _harvestGatewayStub, _harvestGatewayStub, _clock);
                var response = getBillablePeople.Execute();
                
                response.BillablePeople.Count.Should().Be(1);
                response.BillablePeople.First().Id.Should().Be("U8723");
            }
        }
    }
}
