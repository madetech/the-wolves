using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class RemindUserTests
    {
        class MessageSenderSpy : IMessageSender
        {
            public Message Message;
        
            public void Send(Message message)
            {
                Message = message;
            }
        }
       
        [Test]
        public void CanRemindUser()
        {
            var spy = new MessageSenderSpy();
            var remindUser = new RemindUser(spy);
            
            remindUser.Execute(new RemindUserRequest
            {
                UserId = "U120123D"
            });
            
            spy.Message.UserId.Should().Be("U120123D");
        }
        
        [Test]
        public void CanRemindUser2()
        {
            var spy = new MessageSenderSpy();
            var remindUser = new RemindUser(spy);
            
            remindUser.Execute(new RemindUserRequest
            {
                UserId = "U87219HAW"
            });
            
            spy.Message.UserId.Should().Be("U87219HAW");
        }
    }
}