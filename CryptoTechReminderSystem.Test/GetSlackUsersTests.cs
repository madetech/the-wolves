using System.Reflection.PortableExecutable;
using CryptoTechReminderSystem.UseCase;
using NUnit.Framework;
using FluentAssertions;

namespace CryptoTechReminderSystem.Test
{
    public class GetSlackUsersTests
    {
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        public void CanGetOneUser()
        {
            var getSlackUsers = new GetSlackUsers();
            var response = getSlackUsers.Execute();
            response.Should().HaveCount(1);
        }
        
    }
}