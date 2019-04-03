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
        class SlackGatewaySpy : IMessageSender
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
            var spy = new SlackGatewaySpy();
            var remindUser = new RemindUser(spy);
            
            remindUser.Execute(new RemindUserRequest
            {
                Channel = "U120123D",
                Text = "Please make sure your timesheet is submitted by 13:30 on Friday."
            });
            
            spy.Message.Channel.Should().Be("U120123D");
        }
        
        [Test]
        public void CanRemindUser2()
        {
            var spy = new SlackGatewaySpy();
            var remindUser = new RemindUser(spy);
            
            remindUser.Execute(new RemindUserRequest
            {
                Channel = "U87219HAW",
                Text = "Please make sure your timesheet is submitted by 13:30 on Friday."
            });
            
            spy.Message.Channel.Should().Be("U87219HAW");
        }
    }
}