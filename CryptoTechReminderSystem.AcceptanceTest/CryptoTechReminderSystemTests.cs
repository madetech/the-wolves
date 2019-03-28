using CryptoTechReminderSystem.UseCase;
using NUnit.Framework;
using FluentAssertions;

namespace CryptoTechReminderSystem.AcceptanceTest
{
    public class CryptoTechReminderSystemTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestCanSendDirectMessage()
        {
            var getSlackUsers = new GetSlackUsers();
            var getHarvestUsers = new GetHarvestUsers();
            var findLateUsers = new findLateUsers(getSlackUsers, getHarvestUsers);
            var lateUsers = findLateUsers.Execute();
            var sendDirectMessage = new SendDirectMessage();
            var remindsSlackUsers = new RemindsSlackUsers(sendDirectMessage);
            var response = remindsSlackUsers.Execute(users);

            response.Should().BeTrue();
        }
        
    }
}