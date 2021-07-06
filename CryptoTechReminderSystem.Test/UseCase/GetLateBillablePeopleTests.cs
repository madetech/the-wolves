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
        private static SlackGatewaySpy _slackGatewaySpy;

        [SetUp]
        public void SetUp()
        {
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
                _slackGatewaySpy = new SlackGatewaySpy();
                _clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );

                _getBillablePeople = new GetLateBillablePeople(_slackGatewaySpy, _clock);
            }

            [Test]
            public void CanGetSlackBillablePeople()
            {
                _getBillablePeople.Execute();

                _slackGatewaySpy.IsRetrieveBillablePeopleCalled.Should().BeTrue();
            }
        } 
    }
}
